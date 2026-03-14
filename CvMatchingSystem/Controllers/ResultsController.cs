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
using System.Text.Json;

namespace CvMatchingSystem.Controllers
{
    [Authorize]
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting)
                .OrderByDescending(m => m.CreatedDate);
            return View(await applicationDbContext.ToListAsync());
        }

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

        [Authorize(Roles = "Admin")]
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> ExportToPdf(int id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var result = await _context.MatchingResults
                .Include(m => m.Candidate)
                .Include(m => m.JobPosting)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (result == null) return NotFound();

            string candidateName = result.Candidate?.FullName ?? "Unknown";
            string jobTitle = result.JobPosting?.Title ?? "Unknown";
            
            var matchedSkills = string.IsNullOrEmpty(result.MatchedSkillsJson) 
                ? new List<string>() 
                : JsonSerializer.Deserialize<List<string>>(result.MatchedSkillsJson) ?? new List<string>();
                
            var missingSkills = string.IsNullOrEmpty(result.MissingSkillsJson) 
                ? new List<string>() 
                : JsonSerializer.Deserialize<List<string>>(result.MissingSkillsJson) ?? new List<string>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial").FontColor("#1a1714"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("NEXUS AI").ExtraBold().FontSize(24).FontColor("#c17b3c");
                            col.Item().Text("Candidate Analysis Report").FontSize(10).FontColor(Colors.Grey.Medium).LetterSpacing(0.1f);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"REPORT ID: #{result.Id}").FontSize(9).SemiBold();
                            col.Item().Text($"{result.CreatedDate:f}").FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Background("#f9f7f5").Padding(15).Row(row =>
                        {
                            row.RelativeItem().Column(c => {
                                c.Item().Text("CANDIDATE").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text(candidateName).FontSize(14).Bold();
                            });
                            row.RelativeItem().Column(c => {
                                c.Item().Text("TARGET ROLE").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text(jobTitle).FontSize(14).Bold().FontColor("#c17b3c");
                            });
                        });

                        col.Item().PaddingVertical(10).AlignCenter().Column(c => {
                            c.Item().Text("COMPATIBILITY SCORE").FontSize(10).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().Text($"{result.Score:F2}%").FontSize(64).ExtraBold().FontColor("#c17b3c");
                            c.Item().Text("HYBRID ENSEMBLE CALCULATION").FontSize(7).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().LineHorizontal(1).LineColor("#e2ddd6");

                        col.Item().Column(c => {
                            c.Item().Text("AI VERDICT & EXPLANATION").FontSize(9).SemiBold().FontColor("#c17b3c");
                            c.Item().PaddingTop(5).Text(result.AiExplanation ?? "No explanation provided.").LineHeight(1.5f);
                        });

                        col.Item().Row(row => {
                            row.RelativeItem().PaddingRight(10).Column(c => {
                                c.Item().Text("MATCHED SKILLS").FontSize(9).SemiBold().FontColor(Colors.Green.Medium);
                                foreach(var skill in matchedSkills) c.Item().Text($"- {skill}").FontSize(10);
                            });
                            row.RelativeItem().PaddingLeft(10).Column(c => {
                                c.Item().Text("MISSING SKILLS").FontSize(9).SemiBold().FontColor(Colors.Red.Medium);
                                foreach(var skill in missingSkills) c.Item().Text($"- {skill}").FontSize(10);
                            });
                        });
                    });

                    page.Footer().AlignCenter().Column(c => {
                        c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                        c.Item().PaddingTop(5).Text(x =>
                        {
                            x.Span("This analysis is generated using a combination of LLM Semantic Analysis, Cosine Similarity, and PDF Experience matching.").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"AI_Report_{candidateName.Replace(" ", "_")}.pdf");
        }
    }
}