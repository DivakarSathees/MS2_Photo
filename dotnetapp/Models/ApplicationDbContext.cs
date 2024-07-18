using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<Participant> Participants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workshop>().HasData(
                new Workshop { WorkshopID = 1, Title = "Beginner Photography", Date = DateTime.Now.AddDays(10), Capacity = 10 },
                new Workshop { WorkshopID = 2, Title = "Advanced Photography", Date = DateTime.Now.AddDays(20), Capacity = 10 }
            );
        }
    }
}