using BL;
using BL.ActionResults;
using BL.Authentication;
using BL.CurrencyFlow;
using Domain.Currencies.BaseCurrency;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IAuthenticationManager _authManager;
    private readonly ICurrencyFlowManager _currencyFlowManager;
    private readonly ICrimeManager _crimeManager;
    
    public HomeController(IAuthenticationManager authManager, ICurrencyFlowManager currencyFlowManager, ICrimeManager crimeManager)
    {
        _authManager = authManager;
        _currencyFlowManager = currencyFlowManager;
        _crimeManager = crimeManager;
    }
    
    public async Task<IActionResult> Index()
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (string.IsNullOrEmpty(userKey))
        {
            return RedirectToAction("Manage", "Account");
        }

        User? user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["Feedback"] = "User not found. Please log in again.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Manage", "Account");
        }

        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> ManageCurrency(CurrencyType currencyType, double amount, string operation)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (string.IsNullOrEmpty(userKey))
        {
            TempData["Feedback"] = "Please log in to perform this action.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index");
        }

        var user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["Feedback"] = "User not found. Please log in again.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index");
        }

        try
        {
            if (operation == "add")
            {
                await _currencyFlowManager.AddAmountToUserAsync(user, currencyType, amount);
            }
            else if (operation == "subtract")
            {
                await _currencyFlowManager.SubtractAmountToUserAsync(user, currencyType, amount);
            }

            TempData["Feedback"] = $"{amount} {currencyType} has been successfully {operation}ed.";
            TempData["FeedbackType"] = "success";
        }
        catch (Exception ex)
        {
            TempData["Feedback"] = $"An error occurred: {ex.Message}";
            TempData["FeedbackType"] = "error";
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _authManager.GetAllUsersAsync().Result.Select(user => new
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
            TempData["Feedback"] = "Amount must be greater than zero.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index");
        }

        try
        {
            var user = await _authManager.GetUserByKey(userKey);
            if (user == null)
            {
                TempData["Feedback"] = "User not found.";
                TempData["FeedbackType"] = "error";
                return RedirectToAction("Index");
            }

            await _currencyFlowManager.AddAmountToUserAsync(user, currency, amount);
            TempData["Feedback"] = $"{amount} {currency} added to {user.Name}.";
            TempData["FeedbackType"] = "success";
        }
        catch (Exception ex)
        {
            TempData["Feedback"] = $"An error occurred: {ex.Message}";
            TempData["FeedbackType"] = "error";
        }

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> StealCurrency(CurrencyType currencyType, string walletKey, double amount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (string.IsNullOrEmpty(userKey))
        {
            TempData["Feedback"] = "Please log in to perform this action.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index");
        }

        var user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["Feedback"] = "User not found. Please log in again.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index");
        }
        
        var result = await _crimeManager.StealFromUser(user, walletKey, currencyType, amount);
        
        TempData["Feedback"] = result.Message;
        TempData["FeedbackType"] = result.Success ? "success" : "error";
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> TransferCurrency(CurrencyType currency, string walletKey, double amount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            TempData["Feedback"] = "You must be logged in to transfer currency.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index", "Home");
        }

        var user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["Feedback"] = "User not found.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index", "Home");
        }
        
        var result = await _currencyFlowManager.TransferCurrencyAsync(user, walletKey, currency, amount);

        TempData["Feedback"] = result.Message;
        TempData["FeedbackType"] = result.Success ? "success" : "error";

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ExchangeCurrency(CurrencyType fromCurrency, CurrencyType toCurrency, double amount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            TempData["Feedback"] = "You must be logged in to exchange currency.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index", "Home");
        }

        var user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["Feedback"] = "User not found.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index", "Home");
        }
        
        var result = await _currencyFlowManager.ExchangeCurrency(user, fromCurrency, toCurrency, amount);

        TempData["Feedback"] = result.Message;
        TempData["FeedbackType"] = result.Success ? "success" : "error";

        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> AutoExchangeCurrency()
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (string.IsNullOrEmpty(userKey))
        {
            TempData["Feedback"] = "You must be logged in to auto-exchange.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index", "Home");
        }

        var user = await _authManager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["Feedback"] = "User not found.";
            TempData["FeedbackType"] = "error";
            return RedirectToAction("Index", "Home");
        }

        var result = await _currencyFlowManager.AutoExchangeCurrency(user);

        TempData["Feedback"] = result.Message;
        TempData["FeedbackType"] = result.Success ? "success" : "error";

        return RedirectToAction("Index", "Home");
    }
}
