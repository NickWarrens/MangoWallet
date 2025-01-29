using BL.ActionResults;
using Domain.Users;

namespace BL.Authentication;

public interface IAuthenticationManager
{
    Task<LoginResult> LogInAsync(string passKey);
    Task<SignUpResult> SignUpAsync(string name);
    Task<AccountDeletionResult> DeleteAccount(User user, string accountKey);
    Task<User?> GetUserByKey(string passKey);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task AddUserAsync(string name, string passKey);
}