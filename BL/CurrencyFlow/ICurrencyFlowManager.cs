using BL.ActionResults;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.CurrencyFlow;

public interface ICurrencyFlowManager
{
    Task<TransferResult> TransferCurrencyAsync(User user, string walletKey, CurrencyType currencyType, double amount);
    Task AddAmountToUserAsync(User user, CurrencyType currencyType, double amount);
    Task SubtractAmountToUserAsync(User user, CurrencyType currencyType, double amount);
    Task<ExchangeResult> ExchangeCurrency(User user, CurrencyType currencyToExchange, CurrencyType targetCurrency, double amount);
    Task<ExchangeResult> AutoExchangeCurrency(User user);
}