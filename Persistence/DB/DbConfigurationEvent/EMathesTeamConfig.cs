using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class EMathesTeamConfig : IEntityTypeConfiguration<EMatchesTeam>
    {
        public void Configure(EntityTypeBuilder<EMatchesTeam> builder)
        {
            
            builder.HasKey(ta => new { ta.NameTeam, ta.IdExtremeMatches });

            builder
                .HasOne(org => org.Team)
                .WithMany()
                .HasForeignKey(fk => fk.NameTeam)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(j => j.ExtremeMatch)
                .WithMany()
                .HasForeignKey(j => j.IdExtremeMatches)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}