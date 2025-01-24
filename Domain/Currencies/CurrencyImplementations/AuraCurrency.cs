using Domain.Currencies.BaseCurrency;
namespace Domain.Currencies.CurrencyImplementations;

internal class AuraCurrency : ICurrency
{
    public string Name => "Aura";
    public string Symbol => "Au";
    public double BaseRate => 1;
    public CurrencyType Type => CurrencyType.Aura;

    internal AuraCurrency(){}
}