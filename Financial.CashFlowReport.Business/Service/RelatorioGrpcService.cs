using Financial.CashFlowReport.Business.Interface;
using Financial.CashFlowReport.Server.Protos;

namespace Financial.CashFlowReport.Business.Service
{
    public class RelatorioGrpcService : IRelatorioGrpcService
    {
        private readonly RelatorioService.RelatorioServiceClient _relatorioClient;

        public RelatorioGrpcService(RelatorioService.RelatorioServiceClient relatorioClient) => _relatorioClient = relatorioClient;

        public async Task<RelatorioResponse> ObterRelatorioConsolidadoAsync(string data, CancellationToken cancellationToken)
        {
            var grpcRequest = new RelatorioRequest { Data = data };
            return await _relatorioClient.ObterRelatorioConsolidadoAsync(grpcRequest, cancellationToken: cancellationToken);
        }
    }
}
