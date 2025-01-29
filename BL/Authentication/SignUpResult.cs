namespace BL.ActionResults;

public class SignUpResult : ActionResult
{
    public SignUpResult(bool success, string message) : base(success, message) { }
}