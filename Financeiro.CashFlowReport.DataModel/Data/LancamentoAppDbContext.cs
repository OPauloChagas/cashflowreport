using Microsoft.EntityFrameworkCore;

namespace Financeiro.CashFlowReport.DataModel.Data
{
    public class LancamentoAppDbContext : DbContext
    {
        public LancamentoAppDbContext(DbContextOptions<LancamentoAppDbContext> options)
        : base(options) { }

        public DbSet<LancamentoDataModel> Lancamentos { get; set; }
    }
}
