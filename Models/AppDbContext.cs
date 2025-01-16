using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OVS.Models
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { } 
        public DbSet<ElectionResult> ElectionResults { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectionCandidate> ElectionCandidates { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteHistory> VoteHistories { get; set; }
    }
}
