using Domain.Balances;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public interface ICurrencyBalanceRepository : IRepository<CurrencyBalance>
{
    Task<IEnumerable<CurrencyBalance>> GetByWalletIdAsync(int walletId);
}

public class CurrencyBalanceRepository : BaseRepository<CurrencyBalance>, ICurrencyBalanceRepository
{
    public CurrencyBalanceRepository(MangoWalletDbContext context) : base(context) { }

    public async Task<IEnumerable<CurrencyBalance>> GetByWalletIdAsync(int walletId)
    {
        return await Ctx.CurrencyBalances.Where(cb => cb.UserWalletId == walletId).ToListAsync();
    }
}