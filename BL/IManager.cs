using BL.ActionResults;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL;

public interface IManager
{
    Task<LoginResult> LogInAsync(string passKey);
    Task<SignUpResult> SignUpAsync(string name);
    Task<TransferResult> TransferCurrencyAsync(User user, string walletKey, CurrencyType currencyType, double amount);
    Task AddAmountToUserAsync(User user, CurrencyType currencyType, double amount);
    Task SubtractAmountToUserAsync(User user, CurrencyType currencyType, double amount);
    Task<ExchangeResult> ExchangeCurrency(User user, CurrencyType currencyToExchange,
        CurrencyType targetCurrency, double amount);
    Task<AccountDeletionResult> DeleteAccount(User user, string accountKey);
    Task<User?> GetUserByKey(string passKey);
    Task<IEnumerable<User>> GetAllUsersAsync();
}