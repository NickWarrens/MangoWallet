using System.ComponentModel.DataAnnotations;
using Domain.Currencies.BaseCurrency;

namespace Domain.Balances;

public class CurrencyBalance
{
    public int Id { get; set; }
    
    [Required]
    public CurrencyType CurrencyType { get; set; }
    
    [Range(0, double.MaxValue)]
    public double Balance { get; set; }

    [Required]
    public int UserWalletId { get; set; }
    public UserWallet UserWallet { get; set; }

    public CurrencyBalance(CurrencyType currencyType, double balance = 0)
    {
        CurrencyType = currencyType;
        Balance = balance;
    }

    internal CurrencyBalance()
    { }

    public void AddAmount(double amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount can not be negative");
        }
        Balance += amount;
    }

    public bool SubtractAmount(double amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount can not be negative.");
        }
        if (amount > Balance)
        {
            return false;
        }
        Balance -= amount;
        return true;
    }
}