namespace BL.ActionResults;

public class AccountDeletionResult : ActionResult
{
    public AccountDeletionResult(bool success, string message) : base(success, message)
    {
    }
}