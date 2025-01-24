using BL;
using BL.ActionResults;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace UI.CA;

public class Controller
{
    private IManager _manager;
    private User _currentUser;
    
    public Controller(IManager manager)
    {
        _manager = manager;
    }

    #region Login/Signup
    public void LogInOrSignUp()
{
    while (_currentUser == null)
    {
        Console.WriteLine("\nDo you want to log in or sign up?" +
                          "\n 1. Log in" +
                          "\n 2. Sign up");

        int choice = 0;
        while (choice != 1 && choice != 2)
        {
            string input = Console.ReadLine();

            if (int.TryParse(input, out choice) && (choice == 1 || choice == 2))
            {
                break;
            }

            Console.WriteLine("Enter a valid choice!");
        }

        switch (choice)
        {
            case 1:
                LogIn();
                break;
            case 2:
                SignUp();
                break;
        }
    }
}

private void LogIn()
{
    Console.Write("LOG IN: Enter your account key: ");
    string passkey;

    while (string.IsNullOrWhiteSpace(passkey = Console.ReadLine()))
    {
        Console.WriteLine("Passkey cannot be empty. Please try again:");
    }

    LoginResult result = _manager.LogInAsync(passkey).Result;
    if (result.Success)
    {
        _currentUser = result.User;
        Console.WriteLine(result.Message);
    }
    else
    {
        Console.WriteLine(result.Message);
    }
}

private void SignUp()
{
    Console.Write("SIGN UP: Enter your name: ");
    string name;

    while (string.IsNullOrWhiteSpace(name = Console.ReadLine()))
    {
        Console.WriteLine("Name cannot be empty. Please try again:");
    }

    SignUpResult result = _manager.SignUpAsync(name).Result;
    if (result.Success)
    {
        Console.WriteLine(result.Message);
        LogIn();
    }
    else
    {
        Console.WriteLine(result.Message);
    }
}

public void Actions()
{
    int choice = -1;
    while (choice != 0)
    {
        Console.WriteLine("\nWhat do you want to do?" +
                          "\n0. Exit" +
                          "\n1. Back to login/signup" +
                          "\n2. Show balances" +
                          "\n3. Add currency (AdminOnly)" +
                          "\n4. Subtract currency (AdminOnly)" +
                          "\n5. Transfer currency to another user" +
                          "\n6. Exchange currencies" +
                          "\n7. Delete account");

        while (true)
        {
            string input = Console.ReadLine();

            if (int.TryParse(input, out choice) && (choice >= 0 && choice <= 7))
            {
                break;
            }
            Console.WriteLine("Enter a valid choice!");
        }

        switch (choice)
        {
            case 0:
                Console.WriteLine("\nExiting program...");
                break;
            case 1:
                _currentUser = null;
                LogInOrSignUp();
                break;
            case 2:
                ShowBalances();
                break;
            case 3:
                AddCurrency();
                break;
            case 4:
                SubtractCurrency();
                break;
            case 5:
                TransferCurrency();
                break;
            case 6:
                ExchangeCurrency();
                break;
            case 7:
                DeleteAccount();
                LogInOrSignUp();
                break;
        }
    }
}

    private void AddCurrency()
    {
        if (_currentUser.IsAdmin)
        {
            CurrencyType currencyType = ReadCurrency();
            double amount = ReadDouble("add");
            _manager.AddAmountToUserAsync(_currentUser, currencyType, amount);
        }
        else
        {
            Console.WriteLine("You are not allowed to do this.");
        }
    }

    private void SubtractCurrency()
    {
        if (_currentUser.IsAdmin)
        {
            CurrencyType currencyType = ReadCurrency();
            double amount = ReadDouble("subtract");
            _manager.SubtractAmountToUserAsync(_currentUser, currencyType, amount);
        }
        else
        {
            Console.WriteLine("You are not allowed to do this.");
        }
    }

    private CurrencyType ReadCurrency(string message = "\nPlease select a currency:")
    {
        Console.WriteLine(message);
        foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
        {
            Console.WriteLine($"{(int)currency}. {CurrencyMetaDataProvider.GetCurrencyName(currency)}");
        }

        CurrencyType selectedCurrency;
        
        while (true)
        {
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int selectedNumber) && Enum.IsDefined(typeof(CurrencyType), selectedNumber))
            {
                selectedCurrency = (CurrencyType)selectedNumber;
                break;
            }
            Console.WriteLine("Invalid selection. Please enter a valid number.");
        }
        return selectedCurrency;
    }

    private double ReadDouble(string action)
    {
        double amount;
        
        while (true)
        {
            Console.Write($"Enter the amount you want to {action}: ");
            string? input = Console.ReadLine();
            
            if (double.TryParse(input, out amount) && amount > 0)
            {
                break;
            }
            Console.WriteLine("Invalid amount. Please enter a valid positive number.");
        }

        return amount;
    }

    private void ShowBalances()
    {
        Console.WriteLine("\n" + $"{_currentUser.Name}s wallet:" +
                          "\n==========================================\n" +
                          $"{_currentUser.UserWallet}");
    }

    private void TransferCurrency()
    {
        string walletKey = string.Empty;
        
        while (true)
        {
            Console.Write("Enter the wallet key (12 characters, alphanumeric only): ");
            walletKey = Console.ReadLine();
            
            if (walletKey != null && walletKey.Length == 12 && walletKey.All(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Contains(c)))
            {
                break;
            }

            Console.WriteLine("Invalid wallet key. Please ensure it's exactly 12 characters long and contains only letters and numbers.");
        }
        
        CurrencyType currencyType = ReadCurrency();
        double amount = ReadDouble("transfer");
        TransferResult result = _manager.TransferCurrencyAsync(_currentUser, walletKey, currencyType, amount).Result;
        Console.WriteLine(result.Message);
    }

    public void ExchangeCurrency()
    {
        CurrencyType currencyToExchange = ReadCurrency("\nSelect a currency to exchange:");
        CurrencyType currencyTarget = ReadCurrency("\nSelect a currency you'd like to exchange to:");
        double amount = ReadDouble("exchange");

        ExchangeResult result = _manager.ExchangeCurrency(_currentUser, currencyToExchange, currencyTarget, amount).Result;
        Console.WriteLine(result.Message);
    }

    public void DeleteAccount()
    {
        Console.WriteLine("Are you sure you want to delete your account? (yes/no)");
        string input = Console.ReadLine();
        if (!input.ToLower().Equals("yes"))
        {
            return;
        }

        Console.Write("CONFIRM DELETION: Enter your account key: ");
        string passkey;

        while (string.IsNullOrWhiteSpace(passkey = Console.ReadLine()))
        {
            Console.WriteLine("Passkey cannot be empty. Please try again:");
        }

        AccountDeletionResult result = _manager.DeleteAccount(_currentUser, passkey).Result;
        Console.WriteLine(result.Message);
        _currentUser = null;
    }
    #endregion
}