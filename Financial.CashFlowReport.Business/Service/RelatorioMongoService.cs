using Financial.CashFlowReport.Business.Interface;
using MongoDB.Driver;

namespace Financial.CashFlowReport.Business.Service
{
    public class RelatorioMongoService : IRelatorioMongoService
    {
        private readonly IMongoCollection<RelatorioDiario> _relatorioCollection;
        public RelatorioMongoService(IMongoDatabase mongoDatabase) => _relatorioCollection = mongoDatabase.GetCollection<RelatorioDiario>("relatorios_diarios");
        
        public async Task<RelatorioDiario?> GetRelatorioDiarioByDateAsync(string data)
        {
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, data);
            return await _relatorioCollection.Find(filtro).FirstOrDefaultAsync();
        }

        public async Task InsertOrUpdateLancamentoAsync(LancamentoDataModel lancamento, string data)
        {
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, data);
            var updateLancamento = Builders<RelatorioDiario>.Update.Push(r => r.Lancamentos, lancamento);
            await _relatorioCollection.UpdateOneAsync(filtro, updateLancamento, new UpdateOptions { IsUpsert = true });
        }

        public async Task UpdateTotalLancamentosAsync(string data, double totalCreditos, double totalDebitos)
        {
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, data);
            var updateTotais = Builders<RelatorioDiario>.Update
                .Set(r => r.TotalCreditos, totalCreditos)
                .Set(r => r.TotalDebitos, totalDebitos);
            await _relatorioCollection.UpdateOneAsync(filtro, updateTotais);
        }
    }
}
