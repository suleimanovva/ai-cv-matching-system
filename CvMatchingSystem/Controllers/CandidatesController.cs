using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data;
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Candidates
        public async Task<IActionResult> Index()
        {
            return View(await _context.Candidates.ToListAsync());
        }

        // GET: Candidates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null) return NotFound();

            return View(candidate);
        }

        // GET: Candidates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Candidates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                // Логика загрузки файла через свойство ResumeFile в модели Candidate
                if (candidate.ResumeFile != null && candidate.ResumeFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidate.ResumeFile.FileName);
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/resumes");
                    
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await candidate.ResumeFile.CopyToAsync(stream);
                    }

                    candidate.ResumePath = "resumes/" + fileName;
                }

                _context.Add(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        // GET: Candidates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null) return NotFound();
            
            return View(candidate);
        }

        // POST: Candidates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidate candidate)
        {
            if (id != candidate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (candidate.ResumeFile != null && candidate.ResumeFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidate.ResumeFile.FileName);
                        string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/resumes");
                        
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        string filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await candidate.ResumeFile.CopyToAsync(stream);
                        }

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

        // GET: Candidates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null) return NotFound();

            return View(candidate);
        }

        // POST: Candidates/Delete/5
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