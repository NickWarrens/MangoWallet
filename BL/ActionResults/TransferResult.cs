namespace BL.ActionResults;

public class TransferResult : ActionResult
{
    public TransferResult(bool success, string message) : base(success, message)
    { }
}