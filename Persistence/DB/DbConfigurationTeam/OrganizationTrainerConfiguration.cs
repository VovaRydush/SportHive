using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class OrganizationTrainerConfiguration : IEntityTypeConfiguration<OrganizationTrainer>
    {
        public void Configure(EntityTypeBuilder<OrganizationTrainer> builder)
        {
            builder.HasKey(to => new { to.LoginOrganization, to.LoginTraine });

            builder
                .HasOne(org => org.Organization)
                .WithMany(o => o.OrganizationTrainer)
                .HasForeignKey(fk => fk.LoginOrganization)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(t => t.Trainer)
                .WithMany(to => to.OrganizationTrainer)
                .HasForeignKey(t => t.LoginTraine)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}