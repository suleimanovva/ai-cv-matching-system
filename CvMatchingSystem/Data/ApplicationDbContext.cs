using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.AspNetCore.Identity; 
using CvMatchingSystem.Models;

namespace CvMatchingSystem.Data
{
       public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<MatchingResult> MatchingResults { get; set; }
        
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<JobPosting>()
                .Property(j => j.MinScore)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MatchingResult>()
                .Property(m => m.Score)
                .HasPrecision(18, 2);
        }
    }
}