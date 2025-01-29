using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.ActionResults;

public interface ICrimeManager
{
    Task<StealResult> StealFromUser(User user, string walletKey, CurrencyType currencyType, double amount);
}