using Microsoft.AspNetCore.Mvc.Rendering;

namespace CvMatchingSystem.Models
{
    public class MatchingViewModel
    {
        public IEnumerable<Candidate> Candidates { get; set; } = new List<Candidate>();
        public IEnumerable<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}