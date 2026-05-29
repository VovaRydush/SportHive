using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.SportHive.Domain
{
    public class TeamAthleteConfiguration : IEntityTypeConfiguration<TeamAthlete>
    {
        public void Configure(EntityTypeBuilder<TeamAthlete> builder)
        {
            builder.ToTable("TeamAthlete");

            builder.HasKey(ta => new { ta.NameTeam, ta.loginAthlets });

            builder.Property(ta => ta.NameTeam)
                .HasColumnName("NameTeam");

            builder.Property(ta => ta.loginAthlets)
                .HasColumnName("IdAthlete")
                .HasMaxLength(40);

            builder.Property(ta => ta.AthleteStatus)
                .HasColumnName("AthleteStatus")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .HasOne(ta => ta.Team)
                .WithMany(t => t.TeamAthletes)
                .HasForeignKey(ta => ta.NameTeam)
                .HasPrincipalKey(t => t.TeamName)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(ta => ta.Athlete)
                .WithMany(a => a.TeamAthletes)
                .HasForeignKey(ta => ta.loginAthlets)
                .HasPrincipalKey(a => a.login)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
