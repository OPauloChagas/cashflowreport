using Financial.CashFlowReport.Business.Commands;
using Financial.CashFlowReport.Business.Response;
using Financial.CashFlowReport.Server.Protos;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Financial.CashFlowReport.Business.CommandHandlers
{
    public class ConsolidacaoDiariaCommandHandler : IRequestHandler<ConsolidacaoDiariaCommand, ConsolidacaoDiariaResponse>
    {
        private readonly RelatorioService.RelatorioServiceClient _relatorioClient;
        private readonly IMongoCollection<RelatorioDiario> _relatorioCollection;
        private readonly ILogger<ConsolidacaoDiariaCommandHandler> _logger;
        private readonly RabbitMQConsumer _rabbitMQConsumer;

        public ConsolidacaoDiariaCommandHandler(
            RelatorioService.RelatorioServiceClient relatorioClient,
            IMongoDatabase mongoDatabase,
            ILogger<ConsolidacaoDiariaCommandHandler> logger,
            RabbitMQConsumer rabbitMQConsumer)
        {
            _relatorioClient = relatorioClient;
            _relatorioCollection = mongoDatabase.GetCollection<RelatorioDiario>("relatorios_diarios");
            _logger = logger;
            _rabbitMQConsumer = rabbitMQConsumer;

            // Inicia o consumo de mensagens RabbitMQ assim que o handler é instanciado
            StartListeningForMessages();
        }

        // Método para iniciar o consumo das mensagens RabbitMQ
        public void StartListeningForMessages()
        {
            _logger.LogInformation("Iniciando o consumo de mensagens de RabbitMQ.");
            _rabbitMQConsumer.StartConsuming("queue_consolidacao_diaria", ProcessMessage);
        }

        private void ProcessMessage(string message)
        {
            _logger.LogInformation($"Mensagem recebida de RabbitMQ: {message}");

            try
            {
                var lancamento = JsonConvert.DeserializeObject<LancamentoMessage>(message);
                if (lancamento != null)
                {
                    _logger.LogInformation($"Processando lançamento de ID: {lancamento.Id}");

                    var novoLancamento = new LancamentoDataModel
                    {
                        Id = lancamento.Id, 
                        Tipo = lancamento.Tipo,
                        Valor = lancamento.Valor,
                        Descricao = lancamento.Descricao,
                        Data = lancamento.Data
                    };

                    var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, lancamento.Data);

                    var updateLancamento = Builders<RelatorioDiario>.Update.Push(r => r.Lancamentos, novoLancamento);

                    _relatorioCollection.UpdateOne(filtro, updateLancamento, new UpdateOptions { IsUpsert = true });

                    var relatorio = _relatorioCollection.Find(filtro).FirstOrDefault();
                    if (relatorio != null)
                    {
                        var totalCreditos = relatorio.Lancamentos.Where(l => l.Tipo == "credito").Sum(l => l.Valor);
                        var totalDebitos = relatorio.Lancamentos.Where(l => l.Tipo == "debito").Sum(l => l.Valor);

                        var updateTotais = Builders<RelatorioDiario>.Update
                            .Set(r => r.TotalCreditos, totalCreditos)
                            .Set(r => r.TotalDebitos, totalDebitos);

                        _relatorioCollection.UpdateOne(filtro, updateTotais);
                    }

                    _logger.LogInformation("Lançamento e totais salvos com sucesso no MongoDB.");
                }
                else
                {
                    _logger.LogWarning("Mensagem deserializada é nula.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem recebida de RabbitMQ.");
            }
        }


        public async Task<ConsolidacaoDiariaResponse> Handle(ConsolidacaoDiariaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, request.Data);
                var relatorioDiario = await _relatorioCollection.Find(filtro).FirstOrDefaultAsync(cancellationToken);

                if (relatorioDiario == null)
                {
                    _logger.LogInformation("Dados não encontrados no MongoDB. Consultando serviço gRPC e consolidando no PostgreSQL.");

                    var grpcRequest = new RelatorioRequest { Data = request.Data };
                    var grpcResponse = await _relatorioClient.ObterRelatorioConsolidadoAsync(grpcRequest, cancellationToken: cancellationToken);

                    var consolidacaoResponse = new ConsolidacaoDiariaResponse
                    {
                        Data = grpcResponse.Data,
                        SaldoTotal = grpcResponse.SaldoTotal,
                        Sucesso = true,
                        Lancamentos = grpcResponse.RelatorioLancamentos.Select(l => new LancamentoDto
                        {
                            Id = l.Id.ToString(),
                            Tipo = l.Tipo,
                            Valor = l.Valor,
                            Descricao = l.Descricao,
                            Data = l.Data
                        }).ToList()
                    };

                    var lancamentosConsolidados = grpcResponse.RelatorioLancamentos.Select(l => new LancamentoDataModel
                    {
                        Id = Guid.NewGuid(), // Usando o mesmo Id do gRPC
                        Tipo = l.Tipo,
                        Valor = l.Valor,
                        Descricao = l.Descricao,
                        Data = l.Data
                    }).ToList();

                    var totalCreditos = lancamentosConsolidados.Where(l => l.Tipo == "credito").Sum(l => l.Valor);
                    var totalDebitos = lancamentosConsolidados.Where(l => l.Tipo == "debito").Sum(l => l.Valor);

                    var novoRelatorioDiario = new RelatorioDiario
                    {
                        Data = grpcResponse.Data,
                        TotalCreditos = totalCreditos,
                        TotalDebitos = totalDebitos,
                        Lancamentos = lancamentosConsolidados
                    };

                    await _relatorioCollection.InsertOneAsync(novoRelatorioDiario, cancellationToken);

                    return consolidacaoResponse;
                }

                // Calcula o saldo consolidado a partir do MongoDB
                var saldoConsolidado = relatorioDiario.TotalCreditos - relatorioDiario.TotalDebitos;

                return new ConsolidacaoDiariaResponse
                {
                    Data = relatorioDiario.Data,
                    SaldoTotal = saldoConsolidado,
                    Sucesso = true,
                    Lancamentos = relatorioDiario.Lancamentos.Select(l => new LancamentoDto
                    {
                        Id = l.Id.ToString(),
                        Tipo = l.Tipo,
                        Valor = l.Valor,
                        Descricao = l.Descricao,
                        Data = l.Data
                    }).ToList()
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao consultar relatório consolidado: {Message}", rpcEx.Message);
                return new ConsolidacaoDiariaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar relatório consolidado."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consolidar relatório: {Message}", ex.Message);
                return new ConsolidacaoDiariaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro inesperado ao consolidar relatório."
                };
            }
        }

    }
}
