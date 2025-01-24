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
}