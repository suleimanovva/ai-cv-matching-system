using System.ComponentModel.DataAnnotations;

namespace CvMatchingSystem.Models
{
    public class DashboardViewModel
    {
              public int TotalCandidates { get; set; }

        public int TotalJobPostings { get; set; }

        public int TotalMatches { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double AverageMatchScore { get; set; }

        public List<MatchingResult>? RecentMatches { get; set; }
    }
}