using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Team");

            builder.HasKey(t => t.TeamName);

            builder.Property(t => t.TeamName)
                .HasColumnName("TeamName");

            builder.Property(t => t.TeamPhoto)
                .HasColumnName("TeamPhoto")
                .IsRequired(false);

            builder.Property(t => t.LoginTrainer)
                .HasColumnName("LoginTrainer")
                .HasMaxLength(40)
                .IsRequired();

            builder.Property(t => t.TypeSport)
                .HasColumnName("TypeSport")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .HasOne(t => t.Trainer)
                .WithOne()
                .HasForeignKey<Team>(team => team.LoginTrainer)
                .HasPrincipalKey<Trainer>(trainer => trainer.login)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
