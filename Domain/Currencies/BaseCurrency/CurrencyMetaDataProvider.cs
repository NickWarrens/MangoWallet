namespace Domain.Currencies.BaseCurrency;

public static class CurrencyMetaDataProvider
{
    private static readonly Dictionary<CurrencyType, ICurrency> _currencyCache = new Dictionary<CurrencyType, ICurrency>();
    
    private static ICurrency GetICurrency(CurrencyType currencyType)
    {
        if (!_currencyCache.ContainsKey(currencyType))
        {
            _currencyCache[currencyType] = CurrencyFactory.CreateCurrency(currencyType);
        }
        return _currencyCache[currencyType];
    }
    
    public static string GetCurrencyName(CurrencyType currencyType)
    {
        ICurrency currency = GetICurrency(currencyType);
        return currency.Name;
    }

    public static string GetCurrencySymbol(CurrencyType currencyType)
    {
        ICurrency currency = GetICurrency(currencyType);
        return currency.Symbol;
    }

    public static double GetCurrencyBaseRate(CurrencyType currencyType)
    {
        ICurrency currency = GetICurrency(currencyType);
        return currency.BaseRate;
    }
}