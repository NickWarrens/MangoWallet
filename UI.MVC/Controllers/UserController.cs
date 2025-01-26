using BL;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class UserController : Controller
{
    private readonly IManager _manager;

    public UserController(IManager manager)
    {
        _manager = manager;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _manager.GetAllUsersAsync();
        return View(users);
    }
}