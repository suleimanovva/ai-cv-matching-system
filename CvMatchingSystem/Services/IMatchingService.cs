namespace CvMatchingSystem.Services;

public interface IMatchingService
{
    double CalculateMatchScore(string resumeText, string jobRequirements);
}