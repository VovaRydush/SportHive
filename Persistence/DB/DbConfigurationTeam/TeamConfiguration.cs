using DB.SportHive.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SportHive.DbConfiguration
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Team");

            builder.HasKey(t => t.TeamName);

            builder.Property(t => t.TeamName)
                .HasColumnName("TeamName")
                .IsRequired();

            builder.Property(t => t.LoginTrainer)
                .HasColumnName("LoginTrainer")
                .HasMaxLength(40)
                .IsRequired();

            builder.Property(t => t.TypeSport)
                .HasColumnName("TypeSport")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.TeamPhoto)
                .HasColumnName("TeamPhoto")
                .IsRequired(false);

            builder
                .HasOne(t => t.Trainer)
                .WithMany(t => t.Teams)
                .HasForeignKey(t => t.LoginTrainer)
                .HasPrincipalKey(t => t.login)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
