using System.ComponentModel.DataAnnotations.Schema; 
using Microsoft.AspNetCore.Http;

namespace CvMatchingSystem.Models;

public class Candidate
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    
    public string? ResumePath { get; set; }
    
    public int ExperienceYears { get; set; }

  
    [NotMapped] 
    public IFormFile? ResumeFile { get; set; }
}