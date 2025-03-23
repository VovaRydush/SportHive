using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace DB.SportHive.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserdDtails> UserdDtails { get; set; }
        public DbSet<RefreshToken> refrehsToken { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserdDtails>()
            .HasOne(ud => ud.User)
            .WithOne(u => u.UserdDtails)
            .HasForeignKey<UserdDtails>(ud => ud.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
