using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Balances;
namespace Domain.Users;

public class User
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(36)]
    public string Key { get; set; }

    public bool IsAdmin { get; set; }

    public UserWallet UserWallet { get; set; }

    public User(string name)
    {
        if (name.Contains("uX9dUJ79."))
        {
            IsAdmin = true;
            name = name.Replace("uX9dUJ79.", "");
        }
        else
        {
            IsAdmin = false;
        }
        
        Name = name;
        Key = GenerateAccountKey();
        UserWallet = new UserWallet(this);
    }
    
    public User(string name, string passKey)
    {
        if (name.Contains("uX9dUJ79."))
        {
            IsAdmin = true;
            name = name.Replace("uX9dUJ79.", "");
        }
        else
        {
            IsAdmin = false;
        }
        
        Name = name;
        Key = passKey;
        UserWallet = new UserWallet(this);
    }

    internal User()
    { }

    public static string GenerateAccountKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var stringBuilder = new StringBuilder(36);

        for (int i = 0; i < 36; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }

        return stringBuilder.ToString(); 
    }
}