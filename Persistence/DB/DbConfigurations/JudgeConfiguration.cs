using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class JudgeConfiguration : IEntityTypeConfiguration<Judge>
    {
        public void Configure(EntityTypeBuilder<Judge> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder
             .HasOne(a => a.User)
             .WithOne()
             .HasForeignKey<Judge>(a => a.Id)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}