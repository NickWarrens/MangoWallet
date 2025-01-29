using BL.ActionResults;
using DAL;
using Domain.Users;

namespace BL.Authentication;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly IUserRepository _userRepository;

    public AuthenticationManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResult> LogInAsync(string passKey)
    {
        User? user = await _userRepository.GetByAccountKeyAsync(passKey);
        if (user == null)
            return new LoginResult(false, null, "Invalid passkey. User not found.");

        return new LoginResult(true, user, $"Welcome back, {user.Name}");
    }

    public async Task<SignUpResult> SignUpAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new SignUpResult(false, "Name cannot be empty.");

        User user;
        do
        {
            user = new User(name);
        } while (await _userRepository.GetByAccountKeyAsync(user.Key) != null);

        await _userRepository.AddAsync(user);
        return new SignUpResult(true, $"{user.Key}");
    }

    public async Task<AccountDeletionResult> DeleteAccount(User user, string accountKey)
    {
        if (!user.Key.Equals(accountKey))
            return new AccountDeletionResult(false, "Account key mismatch.");

        await _userRepository.DeleteAsync(user.Id);
        return new AccountDeletionResult(true, $"Account {user.Name} deleted successfully.");
    }

    public async Task<User?> GetUserByKey(string passKey)
    {
        return await _userRepository.GetByAccountKeyAsync(passKey);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersWithDetails();
    }

    public async Task AddUserAsync(string name, string passKey)
    {
        User user = new User(name, passKey);
        await _userRepository.AddAsync(user);
    }
}