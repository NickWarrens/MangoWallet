using BL.ActionResults;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.CasinoManager;

public interface ICasinoManager
{
    Task<CoinFlipResult> PerformCoinFlipAsync(User user, double betAmount, CurrencyType currencyType, string bet);
}