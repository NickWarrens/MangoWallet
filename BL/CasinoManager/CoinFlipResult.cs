namespace BL.ActionResults;

public class CoinFlipResult : ActionResult
{
    public double Amount;

    public CoinFlipResult(bool success, string message, double amount) : base(success, message)
    {
        Amount = amount;
    }
}