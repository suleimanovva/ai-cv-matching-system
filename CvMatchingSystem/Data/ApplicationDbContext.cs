using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        // Указываем EF Core, какие таблицы нам нужны в базе данных:
        public DbSet<User> Users { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<MatchingResult> MatchingResults { get; set; }
    }
}