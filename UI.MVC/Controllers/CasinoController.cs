using BL;
using Domain.Balances;
using Domain.Currencies.BaseCurrency;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class CasinoController : Controller
{
    private readonly IManager _manager;

    public CasinoController(IManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (string.IsNullOrEmpty(userKey))
        {
            Console.WriteLine("UserKey is null or empty in session.");
            return RedirectToAction("Manage", "Account");
        }

        var user = await _manager.GetUserByKey(userKey);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> CoinFlip(double betAmount, CurrencyType currencyType, string bet)
    {
        var user = await _manager.GetUserByKey(HttpContext.Session.GetString("UserKey"));
        if (user == null)
        {
            return RedirectToAction("Manage", "Account");
        }

        var result = await _manager.PerformCoinFlipAsync(user, betAmount, currencyType, bet);

        TempData["FlipResult"] = result.Message;
        TempData["ResultType"] = result.Success ? "win" : "lose";
        
        return RedirectToAction("Index");
    }
}