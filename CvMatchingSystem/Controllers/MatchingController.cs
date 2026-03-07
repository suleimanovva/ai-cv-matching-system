using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CvMatchingSystem.Models;
using CvMatchingSystem.Services;
using CvMatchingSystem.Data; 
using System.Linq;
using System.Threading.Tasks;
using System.IO; // ДОБАВЛЕНО: Обязательно для работы с путями (Path, Directory)

namespace CvMatchingSystem.Controllers
{
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
        public IActionResult Index()
        {
            ViewBag.Candidates = new SelectList(_context.Candidates, "Id", "FullName");
            ViewBag.JobPostings = new SelectList(_context.JobPostings, "Id", "Title");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(int candidateId, int jobId)
        {
            // 1. Получаем кандидата из базы
            var candidate = _context.Candidates.Find(candidateId);
            if (candidate == null || string.IsNullOrEmpty(candidate.ResumePath))
            {
                ViewBag.Error = "Candidate not found or has no resume.";
                return View("Index"); 
            }

            // 2. Получаем вакансию из базы
            var job = _context.JobPostings.Find(jobId);
            if (job == null)
            {
                ViewBag.Error = "Job posting not found.";
                return View("Index");
            }

            // --- ИСПРАВЛЕННЫЙ БЛОК: ЧТЕНИЕ PDF ---
            // Формируем полный путь к PDF файлу (папка проекта + wwwroot + путь из базы)
            string fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", candidate.ResumePath);

            // Читаем весь текст из PDF с помощью твоего NLP-метода
            string extractedResumeText = _matchingService.ExtractTextFromPdf(fullFilePath);

            // Теперь передаем в калькулятор НАСТОЯЩИЙ текст резюме, а не просто ссылку
            double calculatedScore = _matchingService.CalculateMatchScore(extractedResumeText, job.Requirements ?? "");
            // --------------------------------------

            // 4. Создаем объект результата и сохраняем его в базу
            var matchResult = new MatchingResult
            {
                CandidateId = candidate.Id,
                JobId = job.Id,
                Score = (decimal)calculatedScore 
            };

            _context.MatchingResults.Add(matchResult);
            await _context.SaveChangesAsync(); 

            // 5. Передаем данные во View для отображения
            ViewBag.Score = calculatedScore;
            ViewBag.CandidateName = candidate.FullName;
            ViewBag.JobTitle = job.Title;

            // Заново заполняем списки, чтобы форма отображалась корректно
            ViewBag.Candidates = new SelectList(_context.Candidates, "Id", "FullName");
            ViewBag.JobPostings = new SelectList(_context.JobPostings, "Id", "Title");

            return View("Index");
        }
    }
}