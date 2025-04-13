using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(t => t.Id);
            
            builder
            .HasOne(t => t.Trainer)
            .WithOne()
            .HasForeignKey<Team>(t => t.IdTraine)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}