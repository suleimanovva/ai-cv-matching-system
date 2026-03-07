using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data;
using CvMatchingSystem.Models;
using Microsoft.AspNetCore.Authorization; 
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CvMatchingSystem.Controllers
{
    [Authorize] // Все методы доступны только залогиненным пользователям
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Results
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MatchingResults.Include(m => m.Candidate).Include(m => m.JobPosting);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Results/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var matchingResult = await _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (matchingResult == null) return NotFound();

            return View(matchingResult);
        }

        // GET: Results/Create
        public IActionResult Create()
        {
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Id");
            ViewData["JobId"] = new SelectList(_context.JobPostings, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CandidateId,JobId,Score,CreatedDate")] MatchingResult matchingResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(matchingResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Id", matchingResult.CandidateId);
            ViewData["JobId"] = new SelectList(_context.JobPostings, "Id", "Id", matchingResult.JobId);
            return View(matchingResult);
        }

        // GET: Results/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var matchingResult = await _context.MatchingResults.FindAsync(id);
            if (matchingResult == null) return NotFound();
            
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Id", matchingResult.CandidateId);
            ViewData["JobId"] = new SelectList(_context.JobPostings, "Id", "Id", matchingResult.JobId);
            return View(matchingResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,JobId,Score,CreatedDate")] MatchingResult matchingResult)
        {
            if (id != matchingResult.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matchingResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchingResultExists(matchingResult.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Id", matchingResult.CandidateId);
            ViewData["JobId"] = new SelectList(_context.JobPostings, "Id", "Id", matchingResult.JobId);
            return View(matchingResult);
        }

        // ==========================================
        // 🛡️ ТОЛЬКО ДЛЯ АДМИНИСТРАТОРА
        // ==========================================

        // GET: Results/Delete/5
        [Authorize(Roles = "Admin")] // 👈 ЗАМОК
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var matchingResult = await _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (matchingResult == null) return NotFound();

            return View(matchingResult);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // 👈 ЗАМОК
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var matchingResult = await _context.MatchingResults.FindAsync(id);
            if (matchingResult != null)
            {
                _context.MatchingResults.Remove(matchingResult);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchingResultExists(int id)
        {
            return _context.MatchingResults.Any(e => e.Id == id);
        }

        // МЕТОД ЭКСПОРТА В PDF
        public async Task<IActionResult> ExportToPdf(int id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var matchingResult = await _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matchingResult == null)
            {
                return NotFound();
            }

            string candidateName = matchingResult.Candidate?.FullName ?? "Unknown_Candidate";
            string jobTitle = matchingResult.JobPosting?.Title ?? "Unknown_Job";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("AI CV Matching Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);
                            x.Item().Text($"Candidate Name: {candidateName}").FontSize(14); 
                            x.Item().Text($"Job Position: {jobTitle}").FontSize(14); 
                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            x.Item().Text($"Match Score: {matchingResult.Score}%").Bold().FontSize(16).FontColor(Colors.Green.Darken2);
                            x.Item().Text($"Report Generated: {matchingResult.CreatedDate:dd/MM/yyyy HH:mm}");
                        });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            
            return File(pdfBytes, "application/pdf", $"Matching_Report_{candidateName}.pdf");
        }
    }
}