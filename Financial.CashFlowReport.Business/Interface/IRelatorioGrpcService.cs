using Financial.CashFlowReport.Server.Protos;

namespace Financial.CashFlowReport.Business.Interface
{
    public interface IRelatorioGrpcService
    {
        Task<RelatorioResponse> ObterRelatorioConsolidadoAsync(string data, CancellationToken cancellationToken);
    }

}
