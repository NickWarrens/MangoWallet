using BL;
using Domain.Currencies.BaseCurrency;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IManager _manager;

    public HomeController(IManager manager)
    {
        _manager = manager;
    }
    
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("UserKey") == null)
        {
            return RedirectToAction("Manage", "Account");
        }

        var user = _manager.GetUserByKey(HttpContext.Session.GetString("UserKey")).Result;
        return View(user);
    }
    
    [HttpPost]
    public IActionResult ManageCurrency(CurrencyType currencyType, double amount, string operation)
    {
        var user = _manager.GetUserByKey(HttpContext.Session.GetString("UserKey")).Result;

        try
        {
            if (operation == "add")
            {
                _manager.AddAmountToUserAsync(user, currencyType, amount);
            }
            else if (operation == "subtract")
            { 
                _manager.SubtractAmountToUserAsync(user, currencyType, amount);
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
    public IActionResult AddCurrencyToUser(string userKey, CurrencyType currency, double amount)
    {
        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount must be greater than zero.";
            return RedirectToAction("Index");
        }

        try
        {
            var user = _manager.GetUserByKey(userKey).Result;
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            _manager.AddAmountToUserAsync(user, currency, amount);
            TempData["SuccessMessage"] = $"{amount} {currency} added to {user.Name}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}