using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // ДОБАВЛЕНО
using Microsoft.AspNetCore.Identity; // ДОБАВЛЕНО
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Data
{
    // МЫ МЕНЯЕМ DbContext НА IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        // Твои таблицы для бизнеса
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<MatchingResult> MatchingResults { get; set; }
        
        // ПРИМЕЧАНИЕ: DbSet<User> больше не нужен, 
        // так как IdentityDbContext уже содержит таблицу пользователей внутри себя.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ЭТА СТРОКА КРИТИЧЕСКИ ВАЖНА

            // Убираем те самые желтые предупреждения (warn) про decimal из терминала
            modelBuilder.Entity<JobPosting>()
                .Property(j => j.MinScore)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MatchingResult>()
                .Property(m => m.Score)
                .HasPrecision(18, 2);
        }
    }
}