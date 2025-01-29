using BL;
using BL.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class UserController : Controller
{
    private readonly IAuthenticationManager _authManager;

    public UserController(IAuthenticationManager authManager)
    {
        _authManager = authManager;
    }

    public async Task<IActionResult> Users()
    {
        if (HttpContext.Session.GetString("UserKey") == null)
        {
            return RedirectToAction("Manage", "Account");
        }
        
        var users = await _authManager.GetAllUsersAsync();
        return View(users);
    }
}