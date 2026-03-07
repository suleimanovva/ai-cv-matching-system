using UglyToad.PdfPig;
using System.Text;
using System.Text.RegularExpressions;

namespace CvMatchingSystem.Services;

public class MatchingService : IMatchingService 
{
    // 1. Карта синонимов для унификации навыков
    private readonly Dictionary<string, string> _synonyms = new Dictionary<string, string>
    {
        { "csharp", "c#" },
        { "c-sharp", "c#" },
        { "dot net", ".net" },
        { "dotnet", ".net" },
        { "ml", "machine learning" },
        { "ai", "artificial intelligence" },
        { "js", "javascript" },
        { "reactjs", "react" }
    };

    private readonly HashSet<string> _stopWords = new HashSet<string> 
    { 
        "and", "the", "with", "from", "for", "this", "that", "about", 
        "using", "your", "will", "our", "all", "highly", "work", "team"
    };

    public string ExtractTextFromPdf(string filePath)
    {
        if (!File.Exists(filePath)) return "";
        var textBuilder = new StringBuilder();
        try 
        {
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                {
                    textBuilder.Append(page.Text);
                }
            }
        }
        catch { return ""; }
        return textBuilder.ToString();
    }

    public double CalculateMatchScore(string resumeText, string jobRequirements)
    {
        if (string.IsNullOrWhiteSpace(resumeText) || string.IsNullOrWhiteSpace(jobRequirements))
            return 0;

        var resumeWords = GetWords(resumeText);
        var jobWords = GetWords(jobRequirements);

        // Считаем пересечение: сколько слов из вакансии есть в резюме
        var intersectCount = jobWords.Intersect(resumeWords).Count();
        var totalRequirementsCount = jobWords.Count;

        if (totalRequirementsCount == 0) return 0;

        double score = (double)intersectCount / totalRequirementsCount * 100;
        return Math.Round(score, 2);
    }

    public HashSet<string> GetWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new HashSet<string>();

        // Извлекаем токены, поддерживая спецсимволы тех-стека
        var rawMatches = Regex.Matches(text.ToLower(), @"[\w+#.]+") 
                              .Cast<Match>()
                              .Select(m => m.Value);

        var processedWords = new HashSet<string>();

        foreach (var word in rawMatches)
        {
            // Убираем точки в конце (важно для конца предложений)
            var clean = word.TrimEnd('.');

            if (clean.Length <= 1 || _stopWords.Contains(clean)) continue;

            // Если есть синоним — используем основной вариант, иначе оставляем как есть
            var finalWord = _synonyms.ContainsKey(clean) ? _synonyms[clean] : clean;
            processedWords.Add(finalWord);
        }

        return processedWords;
    }
}