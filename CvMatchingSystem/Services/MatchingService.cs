using System.Text.RegularExpressions;

namespace CvMatchingSystem.Services;

public class MatchingService : IMatchingService
{
    public double CalculateMatchScore(string resumeText, string jobRequirements)
    {
        if (string.IsNullOrWhiteSpace(resumeText) || string.IsNullOrWhiteSpace(jobRequirements))
            return 0;

        var resumeWords = GetWords(resumeText);
        var jobWords = GetWords(jobRequirements);

        // Пересечение слов (что совпало)
        var intersectCount = resumeWords.Intersect(jobWords).Count();
        // Объединение слов (все уникальные слова)
        var unionCount = resumeWords.Union(jobWords).Count();

        if (unionCount == 0) return 0;

        return Math.Round((double)intersectCount / unionCount * 100, 2);
    }

    private HashSet<string> GetWords(string text)
    {
        // Убираем пунктуацию, переводим в нижний регистр и берем слова длиннее 2 букв
        return Regex.Matches(text.ToLower(), @"\w+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .Where(w => w.Length > 2)
                    .ToHashSet();
    }
}