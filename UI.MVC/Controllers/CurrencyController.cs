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

    public IActionResult TransferCurrency(CurrencyType currency, string walletKey, double amount)
    {
        // Retrieve the logged-in user
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            ViewBag.ErrorMessage = "You must be logged in to transfer currency.";
            return RedirectToAction("Index", "Home");
        }

        var user = _manager.GetUserByKey(userKey).Result;
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        // Perform the transfer
        var result = _manager.TransferCurrencyAsync(user, walletKey, currency, amount).Result;

        if (result.Success)
        {
            ViewBag.SuccessMessage = result.Message;
        }
        else
        {
            ViewBag.ErrorMessage = result.Message;
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult ExchangeCurrency(CurrencyType fromCurrency, CurrencyType toCurrency, double amount)
    {
        // Retrieve the logged-in user
        var userKey = HttpContext.Session.GetString("UserKey");
        if (userKey == null)
        {
            ViewBag.ErrorMessage = "You must be logged in to exchange currency.";
            return RedirectToAction("Index", "Home");
        }

        var user = _manager.GetUserByKey(userKey).Result;
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        // Perform the exchange
        var result = _manager.ExchangeCurrency(user, fromCurrency, toCurrency, amount).Result;

        if (result.Success)
        {
            ViewBag.SuccessMessage = result.Message;
        }
        else
        {
            ViewBag.ErrorMessage = result.Message;
        }

        return RedirectToAction("Index", "Home");
    }
}