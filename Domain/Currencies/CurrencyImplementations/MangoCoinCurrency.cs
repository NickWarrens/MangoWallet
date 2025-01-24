using Domain.Currencies.BaseCurrency;
namespace Domain.Currencies.CurrencyImplementations;

internal class MangoCoinCurrency : ICurrency
{
    public string Name => "Mango Coin";
    public string Symbol => "Mc";
    public double BaseRate => 1000000;
    public CurrencyType Type => CurrencyType.Mangocoin;
    
    internal MangoCoinCurrency(){}
}