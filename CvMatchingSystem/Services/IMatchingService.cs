using System.Threading.Tasks;
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Services;

public interface IMatchingService
{
    Task<AiMatchingResponse> CalculateMatchScoreAsync(string resumeText, string jobRequirements);
    string ExtractTextFromPdf(string filePath);
}