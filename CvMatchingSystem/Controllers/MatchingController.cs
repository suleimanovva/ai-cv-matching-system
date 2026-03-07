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

    // Улучшенный метод расчета
    [HttpPost]
    public async Task<IActionResult> Calculate(int? candidateId, int? jobId)
    {
        // 1. Если кнопка нажата, а ID пустые — возвращаем на главную, а не в белый экран
        if (candidateId == null || jobId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var candidate = await _context.Candidates.FindAsync(candidateId);
        var job = await _context.JobPostings.FindAsync(jobId);

        if (candidate == null || job == null) return NotFound();

        // 2. Безопасный вызов сервиса (защита от null)
        double score = _matchingService.CalculateMatchScore(
            candidate.FullName ?? "", 
            job.Requirements ?? ""
        );

        // 3. Данные для отображения результата
        ViewBag.Score = score;
        ViewBag.CandidateName = candidate.FullName;
        ViewBag.JobTitle = job.Title;

        // 4. Перезагружаем списки (чтобы они не исчезли со страницы)
        ViewBag.Candidates = new SelectList(await _context.Candidates.ToListAsync(), "Id", "FullName", candidateId);
        ViewBag.Jobs = new SelectList(await _context.JobPostings.ToListAsync(), "Id", "Title", jobId);

        return View("Index");
    }
}