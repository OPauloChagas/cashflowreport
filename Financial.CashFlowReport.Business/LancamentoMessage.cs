namespace Financial.CashFlowReport.Business
{
    public class LancamentoMessage
    {
        public Guid Id { get; set; }
        public string? Tipo { get; set; }
        public double Valor { get; set; }
        public string? Descricao { get; set; }
        public string? Data { get; set; } 
    }

}
