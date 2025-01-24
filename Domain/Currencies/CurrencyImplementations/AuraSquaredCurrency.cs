using Domain.Currencies.BaseCurrency;
namespace Domain.Currencies.CurrencyImplementations;

internal class AuraSquaredCurrency : ICurrency
{
    public string Name => "Aura²";
    public string Symbol => "Au2";
    public double BaseRate => 10000000;
    public CurrencyType Type => CurrencyType.AuraSquared;
    
    internal AuraSquaredCurrency(){}
}