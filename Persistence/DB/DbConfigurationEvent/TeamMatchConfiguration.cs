using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class TeamMatchConfiguration : IEntityTypeConfiguration<TeamMatch>
    {
        public void Configure(EntityTypeBuilder<TeamMatch> builder)
        {
            builder.ToTable("TeamMatch");

            builder.HasKey(tm => tm.IdTeamMatch);
            builder.Property(tm => tm.IdTeamMatch).ValueGeneratedOnAdd();

            builder.HasOne(tm => tm.Event)
                   .WithMany()
                   .HasForeignKey(tm => tm.IdEvent)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(tm => tm.FirstTeam)
                   .WithMany()
                   .HasForeignKey(tm => tm.NameFirstTeam)
                   .OnDelete(DeleteBehavior.NoAction);

                     builder.HasOne(m => m.Judge)
                            .WithMany(j => j.TeamMatch)
                            .HasForeignKey(m => m.loginJudge)
                            .OnDelete(DeleteBehavior.SetNull)
                            .HasPrincipalKey(j => j.login);


            builder.HasOne(tm => tm.SecondTeam)
                   .WithMany()
                   .HasForeignKey(tm => tm.NameSecondTeam)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(tm => tm.DataMatch).HasColumnType("date");
            builder.Property(tm => tm.TimeMatch).HasColumnType("time");


            builder.HasOne(l => l.Location)             
                   .WithMany()                      
                   .HasForeignKey(tm => tm.LocationName)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(tm => tm.Tour);
           
        }
    }
}
