using BL;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IManager _manager;

    public AccountController(IManager manager)
    {
        _manager = manager;
    }

    public async Task<IActionResult> Manage()
    {
        var userKey = HttpContext.Session.GetString("UserKey");

        if (userKey != null)
        {
            var user = await _manager.GetUserByKey(userKey);
            return View("AccountInfo", user);
        }

        return View("LoginSignUp");
    }
    
    public IActionResult SignUp()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> SignUp(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ViewBag.ErrorMessage = "Name cannot be empty";
            return View();
        }

        var result = await _manager.SignUpAsync(name);

        if (result.Success)
        {
            ViewBag.SuccessMessage = result.Message;
            return View();
        }

        ViewBag.ErrorMessage = result.Message;
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string passKey)
    {
        var result = await _manager.LogInAsync(passKey);

        if (result.Success && result.User != null)
        {
            HttpContext.Session.SetString("UserKey", result.User.Key);
            return RedirectToAction("Manage");
        }

        Console.WriteLine(result.Message);
        ViewBag.ErrorMessage = result.Message;
        return View();
    }

    [HttpPost]
    public IActionResult LogOut()
    {
        HttpContext.Session.Remove("UserKey");
        return RedirectToAction("Manage");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(string accountKey)
    {
        var userKey = HttpContext.Session.GetString("UserKey");

        if (userKey == null)
        {
            ViewBag.ErrorMessage = "You must be logged in to delete your account.";
            return RedirectToAction("Manage");
        }

        var user = await _manager.GetUserByKey(userKey);
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return RedirectToAction("Manage");
        }

        if (user.Key != accountKey)
        {
            ViewBag.ErrorMessage = "The passkey you entered does not match your account key.";
            return RedirectToAction("Manage");
        }

        var deletionResult = await _manager.DeleteAccount(user, accountKey);
        if (!deletionResult.Success)
        {
            ViewBag.ErrorMessage = deletionResult.Message;
            return RedirectToAction("Manage");
        }

        HttpContext.Session.Remove("UserKey");
        ViewBag.SuccessMessage = "Your account has been successfully deleted.";
        return RedirectToAction("Index", "Home");
    }
}