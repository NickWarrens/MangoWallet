using BL;
using BL.Authentication;
using BL.CasinoManager;
using Domain.Balances;
using Domain.Currencies.BaseCurrency;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class CasinoController : Controller
{
    private readonly IAuthenticationManager _authManager;
    private readonly ICasinoManager _casinoManager;

    public CasinoController(IAuthenticationManager authManager, ICasinoManager casinoManager)
    {
        _authManager = authManager;
        _casinoManager = casinoManager;
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

        var user = await _authManager.GetUserByKey(userKey);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> CoinFlip(double betAmount, CurrencyType currencyType, string bet)
    {
        var user = await _authManager.GetUserByKey(HttpContext.Session.GetString("UserKey"));
        if (user == null)
        {
            return RedirectToAction("Manage", "Account");
        }

        var result = await _casinoManager.PerformCoinFlipAsync(user, betAmount, currencyType, bet);

        TempData["FlipResult"] = result.Message;
        TempData["ResultType"] = result.Success ? "win" : "lose";
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> OpenLootBox()
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (string.IsNullOrEmpty(userKey))
        {
            return RedirectToAction("Manage", "Account");
        }

        var user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            return RedirectToAction("Manage", "Account");
        }

        var result = await _casinoManager.OpenLootBoxAsync(user);

        TempData["LootBoxResult"] = result.Message;
        TempData["ResultType"] = result.Success ? "win" : "lose";

        return RedirectToAction("Index");
    }
}