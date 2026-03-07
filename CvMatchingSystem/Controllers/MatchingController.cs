using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data;
using CvMatchingSystem.Services;

namespace CvMatchingSystem.Controllers;

public class MatchingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMatchingService _matchingService;

    public MatchingController(ApplicationDbContext context, IMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    // Страница выбора
    public async Task<IActionResult> Index()
    {
        var candidates = await _context.Candidates.ToListAsync();
        var jobs = await _context.JobPostings.ToListAsync();

        ViewBag.Candidates = new SelectList(candidates, "Id", "FullName");
        ViewBag.Jobs = new SelectList(jobs, "Id", "Title");
        
        return View();
    }

   [HttpPost]
public async Task<IActionResult> Calculate(int? candidateId, int? jobId)
{
    if (candidateId == null || jobId == null)
    {
        return RedirectToAction(nameof(Index));
    }

    var candidate = await _context.Candidates.FindAsync(candidateId);
    var job = await _context.JobPostings.FindAsync(jobId);

    if (candidate == null || job == null) return NotFound();

    // --- НАЧАЛО МАГИИ PDF ---

    // 1. Собираем полный путь к файлу. 
    // Мы берем путь к папке проекта + заходим в wwwroot + берем путь из базы (resumes/test_candidate.pdf)
    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", candidate.ResumePath ?? "");

    // 2. Просим сервис вытащить текст из этого файла
    string extractedText = _matchingService.ExtractTextFromPdf(fullPath);

    // 3. Резервный вариант: если PDF пустой или не нашелся, используем имя, чтобы не было ошибки
    string textToCompare = !string.IsNullOrWhiteSpace(extractedText) 
                           ? extractedText 
                           : (candidate.FullName ?? "");

    // 4. Считаем итоговый процент, передавая в метод ВЕСЬ текст из резюме
    double score = _matchingService.CalculateMatchScore(textToCompare, job.Requirements ?? "");

    // --- КОНЕЦ МАГИИ PDF ---

    ViewBag.Score = score;
    ViewBag.CandidateName = candidate.FullName;
    ViewBag.JobTitle = job.Title;

    ViewBag.Candidates = new SelectList(await _context.Candidates.ToListAsync(), "Id", "FullName", candidateId);
    ViewBag.Jobs = new SelectList(await _context.JobPostings.ToListAsync(), "Id", "Title", jobId);

    return View("Index");
}
}