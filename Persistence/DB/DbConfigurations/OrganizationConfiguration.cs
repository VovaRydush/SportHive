using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasIndex(i => i.NameOrganization);
            builder
             .HasOne(a => a.User)
             .WithOne()
             .HasForeignKey<Organization>(a => a.Id);
        }
    }
}