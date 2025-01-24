namespace BL.ActionResults;

public abstract class ActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public ActionResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}