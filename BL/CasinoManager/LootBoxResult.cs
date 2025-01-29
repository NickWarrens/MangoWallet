using BL.ActionResults;

namespace BL.CasinoManager;

public class LootBoxResult : ActionResult
{
    public LootBoxResult(bool success, string message) : base(success, message)
    {
    }
}