using BL.ActionResults;
using DAL;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.CasinoManager;

public class CasinoManager : ICasinoManager
{
    private readonly IUserRepository _userRepository;
    private Random _random = new Random();

    public CasinoManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<CoinFlipResult> PerformCoinFlipAsync(User user, double betAmount, CurrencyType currencyType, string bet)
    {
        if (betAmount > user.UserWallet.GetCurrencyBalance(currencyType))
            return new CoinFlipResult(false, "Not enough balance.", 0);

        var randomValue = new Random().NextDouble();
        string result = randomValue < 0.495 ? "heads" : randomValue < 0.99 ? "tails" : "edge";

        bool isWin = result.Equals(bet.ToLower());
        double amount = result == "edge" ? 10 * betAmount : betAmount;

        string message;
        if (!isWin)
        {
            user.UserWallet.SubtractCurrency(currencyType, betAmount);
            await _userRepository.UpdateAsync(user);
            message = "You lost " + amount + CurrencyMetaDataProvider.GetCurrencySymbol(currencyType) + "...";
        }
        else
        {
            user.UserWallet.AddCurrency(currencyType, amount);
            await _userRepository.UpdateAsync(user);
            message = "You won " + amount + CurrencyMetaDataProvider.GetCurrencySymbol(currencyType) + "!";
        }

        return new CoinFlipResult(isWin, message, amount);
    }

    public async Task<LootBoxResult> OpenLootBoxAsync(User user)
    {
        double currentBalance = user.UserWallet.GetCurrencyBalance(CurrencyType.Aura);
        double d = _random.NextDouble();

        bool isPositive;
        double amount;
        string message;

        if (d <= 0.65) // Positive cases
        {
            isPositive = true;
            d = _random.NextDouble();
            if (d <= 0.005) { amount = Math.Min(currentBalance * 2, double.MaxValue); message = "Your aura has doubled!"; }
            else if (d <= 0.02) { amount = 2500; message = "JACKPOT! You won 2500 aura!"; }
            else if (d <= 0.1) { amount = 1000; message = "You won 1000 aura!"; }
            else if (d <= 0.2) { amount = 500; message = "You won 500 aura!"; }
            else if (d <= 0.4) { amount = 100; message = "You won 100 aura!"; }
            else if (d <= 0.7) { amount = 30; message = "You won 30 aura!"; }
            else { amount = 1; message = "You won 1 aura. Wow..."; }
        
            user.UserWallet.AddCurrency(CurrencyType.Aura, amount);
        }
        else // Negative cases
        {
            isPositive = false;
            d = _random.NextDouble();
            if (d <= 0.03) { amount = currentBalance / 2; message = "Your aura has halved.."; }
            else if (d <= 0.13) { amount = 750; message = "You lost 750 aura. Ouch..."; }
            else if (d <= 0.43) { amount = 500; message = "You lost 500 aura."; }
            else { amount = 50; message = "You lost 50 aura."; }

            if (amount >= currentBalance)
            {
                user.UserWallet.SetCurrencyBalance(CurrencyType.Aura, 0);
            }
            else
            {
                user.UserWallet.SubtractCurrency(CurrencyType.Aura, amount);
            }
        }
        
        await _userRepository.UpdateAsync(user);
        return new LootBoxResult(isPositive, message);
    }
}