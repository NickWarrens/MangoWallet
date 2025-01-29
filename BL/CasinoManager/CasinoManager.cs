using BL.ActionResults;
using DAL;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.CasinoManager;

public class CasinoManager : ICasinoManager
{
    private readonly IUserRepository _userRepository;

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

        if (!isWin)
        {
            user.UserWallet.SubtractCurrency(currencyType, betAmount);
            await _userRepository.UpdateAsync(user);
        }
        else
        {
            user.UserWallet.AddCurrency(currencyType, amount);
            await _userRepository.UpdateAsync(user);
        }

        return new CoinFlipResult(isWin, isWin ? $"You won {amount}!" : $"You lost {betAmount}.", amount);
    }
}