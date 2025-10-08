using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SessionRating>()
             .HasIndex(r => new { r.SessionId, r.UserId })
              .IsUnique();
        }
        public DbSet<Contributor> Contributors { get; set; }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionRating> SessionRatings { get; set; }

    }

}
