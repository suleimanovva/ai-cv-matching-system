using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data; 
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return View();
        }

        var dashboardData = new DashboardViewModel
        {
            TotalCandidates = await _context.Candidates.CountAsync(),
            TotalJobPostings = await _context.JobPostings.CountAsync(),
            TotalMatches = await _context.MatchingResults.CountAsync(),
            
            AverageMatchScore = (double)(await _context.MatchingResults.AnyAsync() 
                ? await _context.MatchingResults.AverageAsync(m => m.Score) 
                : 0),

            RecentMatches = await _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting) 
                .OrderByDescending(m => m.CreatedDate)
                .Take(5)
                .ToListAsync()
        };

        return View(dashboardData);
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