using BL;
using Domain.Currencies.BaseCurrency;
using Microsoft.AspNetCore.Mvc;
using UI.MVC.Models;

namespace UI.MVC.Controllers;

public class CurrencyController : Controller
{
    private readonly IManager _manager;

    public CurrencyController(IManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public IActionResult ManageCurrency()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCurrency(CurrencyType addCurrencyType, double addAmount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            ViewBag.ErrorMessage = "You must be logged in to add currency.";
            return View("ManageCurrency");
        }

        var user = await _manager.GetUserByKey(userKey);
        if (user == null)
        {
            ViewBag.ErrorMessage = "Invalid user session.";
            return View("ManageCurrency");
        }

        try
        {
            await _manager.AddAmountToUserAsync(user, addCurrencyType, addAmount);
            ViewBag.SuccessMessage = $"Successfully added {addAmount} {CurrencyMetaDataProvider.GetCurrencyName(addCurrencyType)}.";
            return View("ManageCurrency");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            return View("ManageCurrency");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SubtractCurrency(CurrencyType subtractCurrencyType, double subtractAmount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            ViewBag.ErrorMessage = "You must be logged in to subtract currency.";
            return View("ManageCurrency");
        }

        var user = await _manager.GetUserByKey(userKey);
        if (user == null)
        {
            ViewBag.ErrorMessage = "Invalid user session.";
            return View("ManageCurrency");
        }

        try
        {
            await _manager.SubtractAmountToUserAsync(user, subtractCurrencyType, subtractAmount);
            ViewBag.SuccessMessage = $"Successfully subtracted {subtractAmount} {CurrencyMetaDataProvider.GetCurrencyName(subtractCurrencyType)}.";
            return View("ManageCurrency");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            return View("ManageCurrency");
        }
    }

    [HttpPost]
    public async Task<IActionResult> TransferCurrency(CurrencyType currency, string walletKey, double amount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to transfer currency.";
            return RedirectToAction("Index", "Home");
        }

        var user = await _manager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }
        
        var result = await _manager.TransferCurrencyAsync(user, walletKey, currency, amount);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ExchangeCurrency(CurrencyType fromCurrency, CurrencyType toCurrency, double amount)
    {
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to exchange currency.";
            return RedirectToAction("Index", "Home");
        }

        var user = await _manager.GetUserByKey(userKey);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }
        
        var result = await _manager.ExchangeCurrency(user, fromCurrency, toCurrency, amount);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction("Index", "Home");
    }
}