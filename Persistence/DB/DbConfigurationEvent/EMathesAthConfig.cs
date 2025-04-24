using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class EMatchesAthleteConfig : IEntityTypeConfiguration<EMatchesAthlete>
    {
        public void Configure(EntityTypeBuilder<EMatchesAthlete> builder)
        {
            
            builder.HasKey(ta => new { ta.loginAthlete, ta.IdExtremeMatches });

            builder
                .HasOne(org => org.Athlete)
                .WithMany(o => o.MatchesAthletes)
                .HasForeignKey(fk => fk.loginAthlete)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(j => j.ExtremeMatch)
                .WithMany(jo => jo.MatchesAthletes)
                .HasForeignKey(j => j.IdExtremeMatches)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}