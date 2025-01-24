using Domain.Currencies.CurrencyImplementations;
namespace Domain.Currencies.BaseCurrency;

internal static class CurrencyFactory
{
    internal static ICurrency CreateCurrency(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Aura: return new AuraCurrency();
            case CurrencyType.Skibidicoin: return new SkibidiCoinCurrency();
            case CurrencyType.Rizzeleer: return new RizzeleerCurrency();
            case CurrencyType.Mangocoin: return new MangoCoinCurrency();
            case CurrencyType.AuraSquared: return new AuraSquaredCurrency();
            default: throw new ArgumentException("Invalid currency type.");
        }
    }
}