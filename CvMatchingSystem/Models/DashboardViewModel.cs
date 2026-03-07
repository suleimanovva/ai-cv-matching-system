using System.ComponentModel.DataAnnotations;

namespace CvMatchingSystem.Models
{
    public class DashboardViewModel
    {
        // Общее количество резюме в системе
        public int TotalCandidates { get; set; }

        // Общее количество вакансий
        public int TotalJobPostings { get; set; }

        // Сколько всего раз был запущен процесс матчинга
        public int TotalMatches { get; set; }

        // Средний балл соответствия по всей системе
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double AverageMatchScore { get; set; }

        // Список последних 5 результатов для таблицы на главной
        public List<MatchingResult>? RecentMatches { get; set; }
    }
}