using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByAccountKeyAsync(string accountKey);
}

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(MangoWalletDbContext context) : base(context) { }

    public async Task<User?> GetByAccountKeyAsync(string accountKey)
    {
        return await Ctx.Users.Include(u => u.UserWallet).ThenInclude(u => u.CurrencyBalances).FirstOrDefaultAsync(u => u.Key == accountKey);
    }
}