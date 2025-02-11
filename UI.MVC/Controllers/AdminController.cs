﻿using BL;
using BL.Authentication;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace UI.MVC.Controllers;

public class AdminController : Controller
{
    private readonly IAuthenticationManager _manager;

    public AdminController(IAuthenticationManager manager)
    {
        _manager = manager;
    }
    
    public async Task<IActionResult> AdminPage()
    {
        var users = await _manager.GetAllUsersAsync();
        return View(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userKey)
    {
        if (string.IsNullOrEmpty(userKey))
        {
            TempData["ErrorMessage"] = "Invalid user key.";
            return RedirectToAction("AdminPage");
        }

        try
        {
            User user = await _manager.GetUserByKey(userKey);
            await _manager.DeleteAccount(user, userKey);
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
        }

        return RedirectToAction("AdminPage");
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(string username, string passkey)
    {
        _manager.AddUserAsync(username, passkey);
        return RedirectToAction("AdminPage");
    }
}