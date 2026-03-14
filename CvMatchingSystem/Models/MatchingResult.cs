using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CvMatchingSystem.Models
{
    public class MatchingResult
    {
        public int Id { get; set; }
        
        public int CandidateId { get; set; }
        
        [ForeignKey("CandidateId")]
        public Candidate? Candidate { get; set; }

        public int JobId { get; set; }
        
        [ForeignKey("JobId")]
        public JobPosting? JobPosting { get; set; }

        public decimal Score { get; set; }

        public string? AiExplanation { get; set; }

        public string? MatchedSkillsJson { get; set; }

        public string? MissingSkillsJson { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}