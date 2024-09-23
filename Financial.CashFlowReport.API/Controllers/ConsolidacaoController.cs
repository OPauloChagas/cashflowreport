using Financial.CashFlowReport.Business.Commands;
using Financial.CashFlowReport.Business.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Financial.CashFlowReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsolidacaoController : ControllerBase
    {
        private readonly ISender _sender;

        public ConsolidacaoController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("consolidacao")]
        public async Task<ActionResult<ConsolidacaoDiariaResponse>> ObterRelatorioConsolidado([FromQuery] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest("A data é obrigatória.");
            }

            var command = new ConsolidacaoDiariaCommand(data);
            var response = await _sender.Send(command);

            if (!response.Sucesso)
            {
                return StatusCode(500, response.Mensagem);
            }

            return Ok(response);
        }
    }

}
