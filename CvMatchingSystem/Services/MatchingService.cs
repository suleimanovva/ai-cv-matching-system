using UglyToad.PdfPig;
using System.Text;
using System.Text.RegularExpressions;

namespace CvMatchingSystem.Services;

public class MatchingService : IMatchingService 
{
    // Вместо списка технологий мы храним только "мусорные" слова
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

    // 1. Находим пересечение (сколько слов из вакансии есть в резюме)
    var intersectCount = jobWords.Intersect(resumeWords).Count();

    // 2. Делим на общее количество слов В ВАКАНСИИ
    // Это покажет: "Какой % требований закрывает кандидат"
    var totalRequirementsCount = jobWords.Count;

    if (totalRequirementsCount == 0) return 0;

    // Рассчитываем процент (например: 3 совпадения из 4 требований = 75%)
    double score = (double)intersectCount / totalRequirementsCount * 100;

    return Math.Round(score, 2);
}

    public HashSet<string> GetWords(string text)
    {
        // Улучшенный Regex: видит C#, .NET, C++ и обычные слова
        return Regex.Matches(text.ToLower(), @"[\w+#.]+") 
                    .Cast<Match>()
                    .Select(m => m.Value)
                    // Оставляем слова длиннее 1 символа и убираем "мусор"
                    .Where(w => w.Length > 1 && !_stopWords.Contains(w)) 
                    .ToHashSet();
    }
}