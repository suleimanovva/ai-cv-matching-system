using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data;
using CvMatchingSystem.Models;
using System.IO; // ДОБАВЛЕНО: Для работы с файлами и папками
using Microsoft.AspNetCore.Http; // ДОБАВЛЕНО: Для типа IFormFile

namespace CvMatchingSystem.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Candidates.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        public IActionResult Create()
        {
            return View();
        }

        // --- ИСПРАВЛЕННЫЙ POST: Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,ExperienceYears")] Candidate candidate, IFormFile ResumeFile)
        {
            if (ModelState.IsValid)
            {
                // Если пользователь выбрал файл
                if (ResumeFile != null && ResumeFile.Length > 0)
                {
                    // Создаем уникальное имя (напр. 550e8400.pdf)
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResumeFile.FileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");

                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ResumeFile.CopyToAsync(stream);
                    }

                    // Записываем путь, который будет храниться в базе
                    candidate.ResumePath = "resumes/" + fileName;
                }

                _context.Add(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        // --- ИСПРАВЛЕННЫЙ POST: Edit ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ResumePath,ExperienceYears")] Candidate candidate, IFormFile? ResumeFile)
        {
            if (id != candidate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Если при редактировании загрузили НОВЫЙ файл
                    if (ResumeFile != null && ResumeFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResumeFile.FileName);
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");
                        
                        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                        
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ResumeFile.CopyToAsync(stream);
                        }

                        // Обновляем путь на новый
                        candidate.ResumePath = "resumes/" + fileName;
                    }

                    _context.Update(candidate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandidateExists(candidate.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(e => e.Id == id);
        }
    }
}