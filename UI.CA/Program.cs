using BL;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace UI.CA;

public class Program
{
    private static IUserRepository _userRepository;
    private static IWalletRepository _userWalletRepository;
    private static ICurrencyBalanceRepository _currencyBalanceRepository;
    private static IManager _manager;
    private static Controller _controller;
    
    public static void Main(string[] args)
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Wallet.db");
        
        var optionsBuilder = new DbContextOptionsBuilder<MangoWalletDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        var context = new MangoWalletDbContext(optionsBuilder.Options);

        _userRepository = new UserRepository(context);
        _userWalletRepository = new WalletRepository(context);
        _currencyBalanceRepository = new CurrencyBalanceRepository(context);
        _manager = new Manager(_userRepository, _userWalletRepository, _currencyBalanceRepository);

        bool isDatabaseCreated = context.CreateDatabase(false);

        if (context.IsDatabaseEmpty())
        {
            //DatabaseSeeder.SeedDatabase(context);
        }

        _controller = new Controller(_manager);
        _controller.LogInOrSignUp();
        _controller.Actions();
    }
}