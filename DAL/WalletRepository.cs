using Domain.Balances;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public interface IWalletRepository : IRepository<UserWallet>
{
    Task<UserWallet> GetByUserIdAsync(int userId);
    Task<UserWallet?> GetByWalletKeyAsync(string walletKey);
}

public class WalletRepository : BaseRepository<UserWallet>, IWalletRepository
{
    public WalletRepository(MangoWalletDbContext context) : base(context) { }

    public async Task<UserWallet> GetByUserIdAsync(int userId)
    {
        return await Ctx.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
    }
    
    public async Task<UserWallet?> GetByWalletKeyAsync(string walletKey)
    {
        return await Ctx.UserWallets.Include(w => w.CurrencyBalances).FirstOrDefaultAsync(w => w.WalletKey == walletKey);
    }
}