using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace Domain.Balances;

public class UserWallet
{
    public int Id { get; set; }
    public ICollection<CurrencyBalance> CurrencyBalances { get; set; }
    public string WalletKey { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    public UserWallet(User user)
    {
        SetUpBalances();
        WalletKey = GenerateWalletKey();
        User = user;
    }

    internal UserWallet()
    { }

    private void SetUpBalances()
    {
        CurrencyBalances = Enum.GetValues(typeof(CurrencyType))
            .Cast<CurrencyType>()
            .Select(currencyType => new CurrencyBalance(currencyType))
            .ToList();
    }

    public void AddCurrency(CurrencyType currencyType, double amount)
    {
        var balance = CurrencyBalances.FirstOrDefault(cb => cb.CurrencyType == currencyType);
        if (balance == null)
        {
            balance = new CurrencyBalance(currencyType);
            CurrencyBalances.Add(balance);
        }
        balance.AddAmount(amount);
    }

    public bool SubtractCurrency(CurrencyType currencyType, double amount)
    {
        var balance = CurrencyBalances.FirstOrDefault(cb => cb.CurrencyType == currencyType);
        if (balance == null)
        {
            throw new InvalidOperationException("Currency not found in wallet.");
        }
        return balance.SubtractAmount(amount);
    }
    
    public double GetCurrencyBalance(CurrencyType currencyType)
    {
        var balance = CurrencyBalances.FirstOrDefault(cb => cb.CurrencyType == currencyType);
        if (balance == null)
        {
            return 0;
        }
        return balance.Balance;
    }
    
    public bool HasSufficientFunds(CurrencyType currencyType, double amount)
    {
        var balance = CurrencyBalances.FirstOrDefault(cb => cb.CurrencyType == currencyType);
        return balance != null && balance.Balance >= amount;
    }
    
    public double GetTotalValueInBaseCurrency()
    {
        double totalValue = 0;
        foreach (var balance in CurrencyBalances)
        {
            var baseRate = CurrencyMetaDataProvider.GetCurrencyBaseRate(balance.CurrencyType);
            totalValue += balance.Balance * baseRate;
        }
        return totalValue;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var balance in CurrencyBalances)
        {
            string currencyName = CurrencyMetaDataProvider.GetCurrencyName(balance.CurrencyType);
            string currencySymbol = CurrencyMetaDataProvider.GetCurrencySymbol(balance.CurrencyType);
            sb.AppendLine($"{currencyName}: {balance.Balance}{currencySymbol}");
        }

        return sb.ToString();
    }
    
    public static string GenerateWalletKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var stringBuilder = new StringBuilder(12);

        for (int i = 0; i < 12; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }

        return stringBuilder.ToString(); 
    }
}