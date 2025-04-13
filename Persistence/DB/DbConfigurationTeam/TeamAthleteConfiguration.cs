using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.SportHive.Domain
{
    public class TeamAthleteConfiguration : IEntityTypeConfiguration<TeamAthlete>
    {
        public void Configure(EntityTypeBuilder<TeamAthlete> builder)
        {
            
            builder.HasKey(ta => new { ta.NameTeam, ta.loginAthlets });

            
            builder
                .HasOne(ta => ta.Team)
                .WithMany(t => t.TeamAthletes) 
                .HasForeignKey(ta => ta.NameTeam) 
                .OnDelete(DeleteBehavior.Cascade); 

            builder
                .HasOne(ta => ta.Athlete)
                .WithMany(a => a.TeamAthletes) 
                .HasForeignKey(ta => ta.loginAthlets) 
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
