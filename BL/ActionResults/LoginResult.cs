using Domain.Users;

namespace BL.ActionResults;

public class LoginResult : ActionResult
{
    public User? User { get; set; }

    public LoginResult(bool success, User? user, string message) : base(success, message)
    {
        User = user;
    }
}