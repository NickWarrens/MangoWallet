using BL.ActionResults;
using DAL;
using Domain.Balances;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL;

public class Manager : IManager
{
    private IUserRepository _userRepository;
    private IWalletRepository _userWallerRepository;
    private ICurrencyBalanceRepository _currencyBalanceRepository;
    private IManager _managerImplementation;

    public Manager(IUserRepository userRepository, IWalletRepository userWallerRepository,
        ICurrencyBalanceRepository currencyBalanceRepository)
    {
        _userRepository = userRepository;
        _userWallerRepository = userWallerRepository;
        _currencyBalanceRepository = currencyBalanceRepository;
    }

    public async Task<LoginResult> LogInAsync(string passKey)
    {
        User? user = await _userRepository.GetByAccountKeyAsync(passKey);

        if (user == null)
        {
            return new LoginResult(false, null, "Invalid passkey. User not found.");
        }

        return new LoginResult(true, user, $"Welcome back, {user.Name}");
    }

    public async Task<SignUpResult> SignUpAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new SignUpResult(false, "Name cannot be empty.");
        }

        User user;
        do
        {
            user = new User(name);
        } while (await _userRepository.GetByAccountKeyAsync(user.Key) != null);

        await _userRepository.AddAsync(user);
        return new SignUpResult(true, $"{user.Key}");
    }

    public async Task AddAmountToUserAsync(User user, CurrencyType currencyType, double amount)
    {
        user.UserWallet.AddCurrency(currencyType, amount);
        await _userWallerRepository.UpdateAsync(user.UserWallet);
    }

    public async Task SubtractAmountToUserAsync(User user, CurrencyType currencyType, double amount)
    {
        user.UserWallet.SubtractCurrency(currencyType, amount);
        await _userWallerRepository.UpdateAsync(user.UserWallet);
    }

    public async Task<TransferResult> TransferCurrencyAsync(User user, string walletKey, CurrencyType currencyType, double amount)
    {
        UserWallet? wallet = await _userWallerRepository.GetByWalletKeyAsync(walletKey);
        if (wallet == null)
        {
            return new TransferResult(false, "Invalid wallet key");
        }
        
        if (!user.UserWallet.SubtractCurrency(currencyType, amount))
        {
            return new TransferResult(false, "Insufficient balance.");
        }
        
        wallet.AddCurrency(currencyType, amount);

        await _userWallerRepository.UpdateAsync(user.UserWallet);
        await _userWallerRepository.UpdateAsync(wallet);

        return new TransferResult(true, $"Successfully transferred {amount}{CurrencyMetaDataProvider.GetCurrencySymbol(currencyType)} to {walletKey}");
    }

    public async Task<ExchangeResult> ExchangeCurrency(User user, CurrencyType currencyToExchange,
        CurrencyType targetCurrency, double amount)
    {
        UserWallet userWallet = user.UserWallet;
        
        if (!user.UserWallet.SubtractCurrency(currencyToExchange, amount))
        {
            return new ExchangeResult(false, "Insufficient balance.");
        }
        double baseRateAmount = amount * CurrencyMetaDataProvider.GetCurrencyBaseRate(currencyToExchange);
        double targetAmount = Math.Round(baseRateAmount / CurrencyMetaDataProvider.GetCurrencyBaseRate(targetCurrency), 2);
        
        userWallet.AddCurrency(targetCurrency, targetAmount);

        await _userWallerRepository.UpdateAsync(userWallet);
        return new ExchangeResult(true,
            $"Successfully exchanged {amount}{CurrencyMetaDataProvider.GetCurrencySymbol(currencyToExchange)} to {targetAmount}{CurrencyMetaDataProvider.GetCurrencySymbol(targetCurrency)}");
    }

    public async Task<AccountDeletionResult> DeleteAccount(User user, string accountKey)
    {
        if (!user.Key.Equals(accountKey))
        {
            return new AccountDeletionResult(false, "Given accountkey did not match user account key.");
        }

        await _userRepository.DeleteAsync(user.Id);
        return new AccountDeletionResult(true, $"Account for {user.Name} with key {user.Key} deleted succesfully.");
    }

    public async Task<User?> GetUserByKey(string passKey)
    {
        return await _userRepository.GetByAccountKeyAsync(passKey);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersWithDetails();
    }

    public async Task AddUserAsync(string name, string passKey)
    {
        User user = new User(name, passKey);
        await _userRepository.AddAsync(user);
    }

    public async Task<CoinFlipResult> PerformCoinFlipAsync(User user, double betAmount, CurrencyType currencyType, string bet)
        {
            if (betAmount > user.UserWallet.GetCurrencyBalance(currencyType))
            {
                return new CoinFlipResult(false, "Not enough balance.", 0);
            }
            
            var randomValue = new Random().NextDouble();
            string result;
            if (randomValue < 0.495)
            {
                result = "heads";
            }
            else if (randomValue < 0.99)
            {
                result = "tails";
            }
            else
            {
                result = "edge";
            }
            if (string.IsNullOrEmpty(bet))
            {
                throw new ArgumentException("Bet cannot be null or empty.");
            }
            
            bool isWin = result.Equals(bet.ToLower());
            string message;
            if (!isWin)
            {
                await SubtractAmountToUserAsync(user, currencyType, betAmount);
                message = "You lost " + betAmount + CurrencyMetaDataProvider.GetCurrencySymbol(currencyType);
                return new CoinFlipResult(isWin, message, betAmount);
            }
            
            double amount = result == "edge"? 10*betAmount : betAmount;
            await AddAmountToUserAsync(user, currencyType, amount);
            message = "You won " + amount + CurrencyMetaDataProvider.GetCurrencySymbol(currencyType);
            return new CoinFlipResult(isWin, message, amount);
        }
}
//5qHfEqWwAVNnzbuaBzJotrCEICtY66cEEW8w