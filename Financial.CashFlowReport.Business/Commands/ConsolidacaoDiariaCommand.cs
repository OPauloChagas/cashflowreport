using Financial.CashFlowReport.Business.Response;
using MediatR;

namespace Financial.CashFlowReport.Business.Commands
{
    public record ConsolidacaoDiariaCommand(string Data) : IRequest<ConsolidacaoDiariaResponse>;

}
