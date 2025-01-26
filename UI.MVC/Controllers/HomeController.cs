using BL;
using Domain.Currencies.BaseCurrency;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IManager _manager;

    public HomeController(IManager manager)
    {
        _manager = manager;
    }
    
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserKey") == null)
        {
            return RedirectToAction("Manage", "Account");
        }

        User? user = await _manager.GetUserByKey(HttpContext.Session.GetString("UserKey"));
        if (user == null)
        {
            return RedirectToAction("Manage", "Account");
        }
        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> ManageCurrency(CurrencyType currencyType, double amount, string operation)
    {
        var user = await _manager.GetUserByKey(HttpContext.Session.GetString("UserKey"));

        try
        {
            if (operation == "add")
            {
                await _manager.AddAmountToUserAsync(user, currencyType, amount);
            }
            else if (operation == "subtract")
            {
                await _manager.SubtractAmountToUserAsync(user, currencyType, amount);
            }

            TempData["SuccessMessage"] = $"{amount} {currencyType} has been successfully {operation}ed.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("Index");
    }

    
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _manager.GetAllUsersAsync().Result.Select(user => new
        {
            key = user.Key,
            name = user.Name,
            walletKey = user.UserWallet.WalletKey
        }).ToList();

        return Json(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCurrencyToUser(string userKey, CurrencyType currency, double amount)
    {
        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount must be greater than zero.";
            return RedirectToAction("Index");
        }

        try
        {
            var user = await _manager.GetUserByKey(userKey);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            await _manager.AddAmountToUserAsync(user, currency, amount);
            TempData["SuccessMessage"] = $"{amount} {currency} added to {user.Name}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}