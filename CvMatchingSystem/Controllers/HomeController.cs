using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // 🧠 УМНЫЙ РЕДИРЕКТ: 
        // Если пользователь авторизован, не показываем ему Welcome-страницу.
        // Сразу отправляем его в гущу событий — к списку кандидатов.
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // "Index" - это метод, "Candidates" - это контроллер
            return RedirectToAction("Index", "Candidates");
        }

        // Если это гость — показываем красивый лендинг
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}