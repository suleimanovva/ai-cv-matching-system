namespace CvMatchingSystem.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Requirements { get; set; }
        public string? Description { get; set; }
        public decimal MinScore { get; set; }
        public int CreatedBy { get; set; } // Ссылка на ID рекрутера (User)
    }
}