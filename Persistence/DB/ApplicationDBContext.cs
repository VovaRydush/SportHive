using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace DB.SportHive.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email) 
                .IsUnique();
        }
    }
}
