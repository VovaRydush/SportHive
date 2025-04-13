using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(t => t.TeamName);
            
            builder
            .HasOne(t => t.Trainer)
            .WithOne()
            .HasForeignKey<Team>(team => team.LoginTrainer)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}