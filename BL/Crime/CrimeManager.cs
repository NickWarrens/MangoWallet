using DAL;
using Domain.Balances;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.ActionResults;

public class CrimeManager : ICrimeManager
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;

    public CrimeManager(IWalletRepository walletRepository, IUserRepository userRepository)
    {
        _walletRepository = walletRepository;
        _userRepository = userRepository;
    }

    public async Task<StealResult> StealFromUser(User user, string walletKey, CurrencyType currencyType, double amount)
    {
        UserWallet? targetWallet = await _walletRepository.GetByWalletKeyAsync(walletKey);
        if (targetWallet == null)
        {
            return new StealResult(false, "The target wallet does not exist.");
        }

        if (amount <= 0)
        {
            return new StealResult(false, "The amount to steal must be greater than 0.");
        }

        if (targetWallet.GetCurrencyBalance(currencyType) < amount)
        {
            return new StealResult(false,
                $"The target wallet does not have enough {CurrencyMetaDataProvider.GetCurrencyName(currencyType)}.");
        }

        if (user.UserWallet.GetCurrencyBalance(currencyType) < amount)
        {
            return new StealResult(false,
                $"You don't have {CurrencyMetaDataProvider.GetCurrencyName(currencyType)} to repay the bank if you got caught!");
        }

        double successChance =
            CalculateStealChance(amount * CurrencyMetaDataProvider.GetCurrencyBaseRate(currencyType));
        Random random = new Random();
        bool isSuccessfulSteal = random.NextDouble() <= successChance;

        if (isSuccessfulSteal)
        {
            targetWallet.SubtractCurrency(currencyType, amount);
            user.UserWallet.AddCurrency(currencyType, amount);

            await _walletRepository.UpdateAsync(targetWallet);
            await _userRepository.UpdateAsync(user);

            return new StealResult(true,
                $"You successfully stole {amount} {CurrencyMetaDataProvider.GetCurrencySymbol(currencyType)}!");
        }

        if (user.UserWallet.GetCurrencyBalance(currencyType) < amount)
        {
            return new StealResult(false,
                $"You got caught, but you don't have enough {CurrencyMetaDataProvider.GetCurrencyName(currencyType)} to lose!");
        }

        user.UserWallet.SubtractCurrency(currencyType, amount);
        await _userRepository.UpdateAsync(user);

        return new StealResult(false,
            $"You got caught! You lost {amount} {CurrencyMetaDataProvider.GetCurrencySymbol(currencyType)}.");
    }
    
    private double CalculateStealChance(double amount)
    {
        const double MaxChance = 0.60;
        const double MinChance = 0.05;
        const double Threshold = 1500.0 / (MaxChance - MinChance);
        const double ScalingFactor = 1.0;
        
        double chance = MaxChance - (amount / Threshold) * ScalingFactor;
        
        Console.WriteLine(Math.Clamp(chance, MinChance, MaxChance));
        return Math.Clamp(chance, MinChance, MaxChance);
    }
}