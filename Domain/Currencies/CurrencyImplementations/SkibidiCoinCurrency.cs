using Domain.Currencies.BaseCurrency;
namespace Domain.Currencies.CurrencyImplementations;

internal class SkibidiCoinCurrency : ICurrency
{
    public string Name => "Skibidi Coin";
    public string Symbol => "Sc";
    public double BaseRate => 1000;
    public CurrencyType Type => CurrencyType.Skibidicoin;
    
    internal SkibidiCoinCurrency(){}
}