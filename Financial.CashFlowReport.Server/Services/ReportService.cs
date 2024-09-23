using Financeiro.CashFlowReport.DataModel.Data;
using Financial.CashFlowReport.Server.Protos;
using Grpc.Core;
using MongoDB.Driver;

namespace Financial.CashFlowReport.Server.Services
{
    public class ReportService : RelatorioService.RelatorioServiceBase
    {
        private readonly IMongoCollection<RelatorioDiario> _relatorioCollection;
        private readonly LancamentoAppDbContext _lancamentoAppDbContext;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IMongoDatabase mongoDatabase, LancamentoAppDbContext lancamentoAppDbContext, ILogger<ReportService> logger)
        {
            _relatorioCollection = mongoDatabase.GetCollection<RelatorioDiario>("relatorios_diarios");
            _lancamentoAppDbContext = lancamentoAppDbContext;
            _logger = logger;
        }

        public override async Task<RelatorioResponse> ObterRelatorioConsolidado(RelatorioRequest request, ServerCallContext context)
        {
            // Buscar no MongoDB
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, request.Data);
            var relatorioDiario = await _relatorioCollection.Find(filtro).FirstOrDefaultAsync();

            // Se não encontrar no MongoDB, buscar no PostgreSQL
            if (relatorioDiario == null)
            {
                _logger.LogInformation("Relatório não encontrado no MongoDB. Consultando PostgreSQL...");

                var lancamentos = _lancamentoAppDbContext.Lancamentos
                    .Where(l => l.Data == request.Data)
                    .ToList();

                if (!lancamentos.Any())
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Relatório não encontrado."));
                }

                var relatorioResponse = new RelatorioResponse
                {
                    Data = request.Data,
                    SaldoTotal = lancamentos.Where(l => l.Tipo == "credito").Sum(l => l.Valor) - lancamentos.Where(l => l.Tipo == "debito").Sum(l => l.Valor),
                };

                foreach (var lancamento in lancamentos)
                {
                    relatorioResponse.RelatorioLancamentos.Add(new Lancamento
                    {
                        Id = lancamento.Id.ToString(),
                        Tipo = lancamento.Tipo,
                        Valor = lancamento.Valor,
                        Descricao = lancamento.Descricao,
                        Data = lancamento.Data
                    });
                }

                return relatorioResponse;
            }

            // Se encontrado no MongoDB, retorna o relatório consolidado
            var response = new RelatorioResponse
            {
                Data = relatorioDiario.Data,
                SaldoTotal = relatorioDiario.TotalCreditos - relatorioDiario.TotalDebitos
            };

            // Adiciona os lançamentos à coleção existente em RelatorioLancamentos
            foreach (var lancamento in relatorioDiario.Lancamentos)
            {
                response.RelatorioLancamentos.Add(new Lancamento
                {
                    Id = lancamento.Id.ToString(),
                    Tipo = lancamento.Tipo,
                    Valor = lancamento.Valor,
                    Descricao = lancamento.Descricao,
                    Data = lancamento.Data
                });
            }

            return response;
        }
    }
}
