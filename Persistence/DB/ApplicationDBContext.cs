using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;
using SportHive.DbConfiguration;
using Microsoft.Extensions.Configuration;

namespace DB.SportHive.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Judge> Judges { get; set; }

        private readonly IConfiguration _configuration;
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options) 
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AthleteConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new JudgeConfiguration());
            modelBuilder.ApplyConfiguration(new TrainerConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Primary"));
        }
    }
}
