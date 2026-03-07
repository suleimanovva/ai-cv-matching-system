using System.ComponentModel.DataAnnotations.Schema; // Нужно для [NotMapped]
using Microsoft.AspNetCore.Http; // Нужно для IFormFile

namespace CvMatchingSystem.Models;

public class Candidate
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    
    // В этой строке мы храним только путь: "resumes/my_file.pdf"
    public string? ResumePath { get; set; }
    
    public int ExperienceYears { get; set; }

    // --- НОВОЕ ПОЛЕ ДЛЯ ЗАГРУЗКИ ---
    
    [NotMapped] // Это говорит базе данных: "Не пытайся создать такую колонку в таблице!"
    public IFormFile? ResumeFile { get; set; }
}