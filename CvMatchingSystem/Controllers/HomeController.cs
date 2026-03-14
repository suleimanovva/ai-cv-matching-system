using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data;
using CvMatchingSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace CvMatchingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // 1. ОТКРЫВАЕТСЯ ПЕРВЫМ (Заставка)
        public IActionResult Index()
        {
            return View();
        }

        // 2. ОТКРЫВАЕТСЯ ТОЛЬКО ПОСЛЕ LOGIN
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            // Выполняем запросы последовательно, чтобы избежать конфликтов в DbContext
            var totalCandidates = await _context.Candidates.CountAsync();
            var totalJobPostings = await _context.JobPostings.CountAsync();
            var totalMatches = await _context.MatchingResults.CountAsync();

            var recentMatches = await _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting)
                .OrderByDescending(m => m.CreatedDate)
                .Take(5)
                .ToListAsync();

            var averageScore = await _context.MatchingResults
                .Select(m => (double?)m.Score)
                .AverageAsync() ?? 0.0;

            var dashboardData = new DashboardViewModel
            {
                TotalCandidates = totalCandidates,
                TotalJobPostings = totalJobPostings,
                TotalMatches = totalMatches,
                AverageMatchScore = averageScore,
                RecentMatches = recentMatches
            };

            return View(dashboardData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}