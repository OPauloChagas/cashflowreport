namespace Financial.CashFlowReport.Business.Interface
{
    public interface IRelatorioMongoService
    {
        Task<RelatorioDiario?> GetRelatorioDiarioByDateAsync(string data);
        Task InsertOrUpdateLancamentoAsync(LancamentoDataModel lancamento, string data);
        Task UpdateTotalLancamentosAsync(string data, double totalCreditos, double totalDebitos);
    }
}
