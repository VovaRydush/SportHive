using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.HasKey(a => a.login);
            
            builder
             .HasOne(a => a.User)
             .WithOne()
             .HasForeignKey<Trainer>(a => a.login)
             .OnDelete(DeleteBehavior.NoAction);

             
        }
    }
}