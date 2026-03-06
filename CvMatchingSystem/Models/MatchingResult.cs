using System;

namespace CvMatchingSystem.Models
{
    public class MatchingResult
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public decimal Score { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}