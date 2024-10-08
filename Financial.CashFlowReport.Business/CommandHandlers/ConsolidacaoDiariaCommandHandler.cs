using Financial.CashFlowReport.Business.Commands;
using Financial.CashFlowReport.Business.Interface;
using Financial.CashFlowReport.Business.Response;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Financial.CashFlowReport.Business.CommandHandlers
{
    public class ConsolidacaoDiariaCommandHandler : IRequestHandler<ConsolidacaoDiariaCommand, ConsolidacaoDiariaResponse>
    {
        private readonly IRelatorioGrpcService _relatorioGrpcService;
        private readonly IRelatorioMongoService _relatorioMongoService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<ConsolidacaoDiariaCommandHandler> _logger;

        public ConsolidacaoDiariaCommandHandler(
            IRelatorioGrpcService relatorioGrpcService,
            IRelatorioMongoService relatorioMongoService,
            IRabbitMQService rabbitMQService,
            ILogger<ConsolidacaoDiariaCommandHandler> logger)
        {
            _relatorioGrpcService = relatorioGrpcService;
            _relatorioMongoService = relatorioMongoService;
            _rabbitMQService = rabbitMQService;
            _logger = logger;

            // Inicia o consumo de mensagens RabbitMQ
            _rabbitMQService.StartListening("queue_consolidacao_diaria", ProcessMessage);
        }

        public void ProcessMessage(string message)
        {
            _logger.LogInformation($"Mensagem recebida de RabbitMQ: {message}");

            try
            {
                var lancamento = JsonConvert.DeserializeObject<LancamentoMessage>(message);
                if (lancamento != null)
                {
                    var novoLancamento = new LancamentoDataModel
                    {
                        Id = lancamento.Id,
                        Tipo = lancamento.Tipo,
                        Valor = lancamento.Valor,
                        Descricao = lancamento.Descricao,
                        Data = lancamento.Data
                    };

                    // Inserir ou atualizar o lançamento no MongoDB
                    _relatorioMongoService.InsertOrUpdateLancamentoAsync(novoLancamento, lancamento.Data).Wait();

                    // Atualizar totais de créditos e débitos no relatório diário
                    var relatorio = _relatorioMongoService.GetRelatorioDiarioByDateAsync(lancamento.Data).Result;
                    if (relatorio != null)
                    {
                        var totalCreditos = relatorio.Lancamentos.Where(l => l.Tipo == "credito").Sum(l => l.Valor);
                        var totalDebitos = relatorio.Lancamentos.Where(l => l.Tipo == "debito").Sum(l => l.Valor);

                        _relatorioMongoService.UpdateTotalLancamentosAsync(lancamento.Data, totalCreditos, totalDebitos).Wait();
                    }
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
                // Busca o relatório diário no MongoDB
                var relatorioDiario = await _relatorioMongoService.GetRelatorioDiarioByDateAsync(request.Data);

                if (relatorioDiario == null)
                {
                    _logger.LogInformation("Dados não encontrados no MongoDB. Consultando serviço gRPC.");

                    // Busca os dados via gRPC
                    var grpcResponse = await _relatorioGrpcService.ObterRelatorioConsolidadoAsync(request.Data, cancellationToken);

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

                    // Monta os lançamentos consolidados
                    var lancamentosConsolidados = grpcResponse.RelatorioLancamentos.Select(l => new LancamentoDataModel
                    {
                        Id = Guid.NewGuid(),
                        Tipo = l.Tipo,
                        Valor = l.Valor,
                        Descricao = l.Descricao,
                        Data = l.Data
                    }).ToList();

                    // Calculos
                    var totalCreditos = lancamentosConsolidados.Where(l => l.Tipo == "credito").Sum(l => l.Valor);
                    var totalDebitos = lancamentosConsolidados.Where(l => l.Tipo == "debito").Sum(l => l.Valor);

                    // Insere ou atualiza o relatório no MongoDB
                    var novoRelatorioDiario = new RelatorioDiario
                    {
                        Data = grpcResponse.Data,
                        TotalCreditos = totalCreditos,
                        TotalDebitos = totalDebitos,
                        Lancamentos = lancamentosConsolidados
                    };

                    await _relatorioMongoService.InsertOrUpdateLancamentoAsync(novoRelatorioDiario.Lancamentos.First(), novoRelatorioDiario.Data);

                    return consolidacaoResponse;
                }

                // Calcula o saldo
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
                _logger.LogError(rpcEx, "Erro ao consultar relatório consolidado.");
                return new ConsolidacaoDiariaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar relatório consolidado."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return new ConsolidacaoDiariaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro inesperado."
                };
            }
        }
    }
}
