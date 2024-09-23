namespace Financial.CashFlowReport.Business.Response
{
    public class ConsolidacaoDiariaResponse
    {
        public string? Data { get; set; }
        public double SaldoTotal { get; set; }
        public List<LancamentoDto> Lancamentos { get; set; } = new List<LancamentoDto>();

        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
    }

    public class LancamentoDto
    {
        public string? Id { get; set; }
        public string? Tipo { get; set; }
        public double Valor { get; set; }
        public string? Descricao { get; set; }
        public string? Data { get; set; }
    }

}
