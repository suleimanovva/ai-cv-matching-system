using System.Collections.Generic;

namespace CvMatchingSystem.Models;

public class AiMatchingResponse
{
    public decimal Score { get; set; }
    public double CandidateYears { get; set; }
    public double RequiredYears { get; set; }
    public decimal FinalHybridScore { get; set; }
    public List<string> MatchedSkills { get; set; } = new();
    public List<string> MissingSkills { get; set; } = new();
    public string Explainability { get; set; } = string.Empty;
}