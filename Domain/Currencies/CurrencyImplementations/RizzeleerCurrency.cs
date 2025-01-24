using Domain.Currencies.BaseCurrency;
namespace Domain.Currencies.CurrencyImplementations;

internal class RizzeleerCurrency : ICurrency
{
    public string Name => "Rizzeleer";
    public string Symbol => "Rz";
    public double BaseRate => 100000;
    public CurrencyType Type => CurrencyType.Rizzeleer;
    
    internal RizzeleerCurrency(){}
}