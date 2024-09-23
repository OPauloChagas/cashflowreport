using Financial.CashFlowReport.Server.Protos;
using Microsoft.Extensions.DependencyInjection;


namespace Financial.CashFlowReport.Sdk.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddGrpcSdk(this IServiceCollection services) 
        {
            services.AddGrpcClient<RelatorioService.RelatorioServiceClient>(client => {

                client.Address = new Uri("https://localhost:7296");            
            });

            services.AddScoped<IReportClient, ReportClient>();
        }
    }
}
