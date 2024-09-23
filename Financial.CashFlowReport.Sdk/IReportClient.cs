using Financial.CashFlowReport.Server.Protos;

namespace Financial.CashFlowReport.Sdk
{
    public interface IReportClient
    {
        Task<RelatorioResponse> ObterRelatorioConsolidadoAsync(RelatorioRequest request, CancellationToken cancellationToken);
    }
}
