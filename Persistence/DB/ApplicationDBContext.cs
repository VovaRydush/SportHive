using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;
using SportHive.DbConfiguration;

namespace DB.SportHive.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Judge> Judges { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }
        public DbSet<OrganizationJudge> OrginizationJudges { get; set; }
        public DbSet<OrganizationTeam> OrganizationTeams { get; set; }
        public DbSet<OrganizationTrainer> OrganizationTrainers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamAthlete> teamAthletes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<IndividualMatch> IndividualMatches { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<TeamMatch> TeamMatches { get; set; }
        public DbSet<EMatchesAthlete> ExtremeMatchesAthetes { get; set; }
        public DbSet<EMatchesTeam> EMatchesTeam { get; set; }
        public DbSet<ExtremeMatch> ExtremeMatches { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AthleteConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new JudgeConfiguration());
            modelBuilder.ApplyConfiguration(new TrainerConfiguration());
            modelBuilder.ApplyConfiguration(new UserPhotoConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationJudgeConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());
            modelBuilder.ApplyConfiguration(new TeamAthleteConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationTeamConfigration());
            modelBuilder.ApplyConfiguration(new OrganizationTrainerConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new IndividualMatchConfig());
            modelBuilder.ApplyConfiguration(new TeamMatchConfiguration());
            modelBuilder.ApplyConfiguration(new ExtremeMatchConfiguration());
            modelBuilder.ApplyConfiguration(new EMatchesAthleteConfig());
            modelBuilder.ApplyConfiguration(new EMathesTeamConfig());
        }

    }
}
