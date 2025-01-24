namespace Domain.Currencies.BaseCurrency;

public interface ICurrency
{
    string Name { get; }
    string Symbol { get; }
    double BaseRate { get; }
    CurrencyType Type { get; }
}