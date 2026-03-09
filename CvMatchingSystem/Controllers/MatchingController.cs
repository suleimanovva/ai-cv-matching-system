using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CvMatchingSystem.Models;
using CvMatchingSystem.Services;
using CvMatchingSystem.Data; 
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO; 
using Microsoft.AspNetCore.Authorization;
using System;

namespace CvMatchingSystem.Controllers
{
    [Authorize]
    public class MatchingController : Controller
    {
        private readonly IMatchingService _matchingService;
        private readonly ApplicationDbContext _context; 

        public MatchingController(IMatchingService matchingService, ApplicationDbContext context)
        {
            _matchingService = matchingService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new MatchingViewModel
            {
                Candidates = await _context.Candidates.ToListAsync(),
                JobPostings = await _context.JobPostings.ToListAsync()
            };
            
            ViewBag.Candidates = new SelectList(viewModel.Candidates, "Id", "FullName");
            ViewBag.JobPostings = new SelectList(viewModel.JobPostings, "Id", "Title");
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(int candidateId, int jobId)
        {
            var candidate = await _context.Candidates.FindAsync(candidateId);
            var job = await _context.JobPostings.FindAsync(jobId);

            if (candidate == null || string.IsNullOrEmpty(candidate.ResumePath) || job == null)
            {
                return RedirectToAction("Index"); 
            }

            string fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", candidate.ResumePath);
            string extractedResumeText = _matchingService.ExtractTextFromPdf(fullFilePath);
            double calculatedScore = _matchingService.CalculateMatchScore(extractedResumeText, job.Requirements ?? "");

            var matchResult = new MatchingResult
            {
                CandidateId = candidate.Id,
                JobId = job.Id,
                Score = (decimal)calculatedScore,
                CreatedDate = DateTime.Now
            };

            _context.MatchingResults.Add(matchResult);
            await _context.SaveChangesAsync(); 

            ViewBag.Score = Math.Round(calculatedScore, 2);
            ViewBag.CandidateName = candidate.FullName;
            ViewBag.JobTitle = job.Title;

            var viewModel = new MatchingViewModel
            {
                Candidates = await _context.Candidates.ToListAsync(),
                JobPostings = await _context.JobPostings.ToListAsync()
            };

            ViewBag.Candidates = new SelectList(viewModel.Candidates, "Id", "FullName");
            ViewBag.JobPostings = new SelectList(viewModel.JobPostings, "Id", "Title");

            return View("Index", viewModel);
        }
    }
}