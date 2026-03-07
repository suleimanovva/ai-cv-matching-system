using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Controllers;

[Authorize(Roles = "Admin")] // 🛡️ ТОЛЬКО ДЛЯ ТЕБЯ
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Список всех пользователей
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    // Форма создания
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Назначаем роль
                await _userManager.AddToRoleAsync(user, model.Role);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }
}