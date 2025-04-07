using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;

namespace SportHive.DbConfiguration
{
    public class AthleteConfiguration : IEntityTypeConfiguration<Athlete>
    {
        public void Configure(EntityTypeBuilder<Athlete> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder
             .HasOne(a => a.User)
             .WithOne()
             .HasForeignKey<Athlete>(a => a.Id)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}