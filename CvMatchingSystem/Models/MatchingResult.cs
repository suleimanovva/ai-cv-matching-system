using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CvMatchingSystem.Models
{
    public class MatchingResult
    {
        public int Id { get; set; }
        
        public int CandidateId { get; set; }
        
        // ДОБАВЛЕНО: Связь с таблицей Кандидатов (чтобы получать FullName)
        [ForeignKey("CandidateId")]
        public Candidate? Candidate { get; set; }

        public int JobId { get; set; }
        
        // ДОБАВЛЕНО: Связь с таблицей Вакансий (чтобы получать Title)
        [ForeignKey("JobId")]
        public JobPosting? JobPosting { get; set; }

        public decimal Score { get; set; }
        
        // ДОБАВЛЕНО: Автоматическое сохранение текущего времени
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}