using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.SportHive.Domain
{
    public class TeamAthleteConfiguration : IEntityTypeConfiguration<TeamAthlete>
    {
        public void Configure(EntityTypeBuilder<TeamAthlete> builder)
        {
            
            builder.HasKey(ta => new { ta.IdTeam, ta.IdAthlete });

            
            builder
                .HasOne(ta => ta.Team)
                .WithMany(t => t.TeamAthletes) 
                .HasForeignKey(ta => ta.IdTeam) 
                .OnDelete(DeleteBehavior.Cascade); 

            builder
                .HasOne(ta => ta.Athlete)
                .WithMany(a => a.TeamAthletes) 
                .HasForeignKey(ta => ta.IdAthlete) 
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
