using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class OrganizationJudgeConfiguration : IEntityTypeConfiguration<OrganizationJudge>
    {
        public void Configure(EntityTypeBuilder<OrganizationJudge> builder)
        {
            builder.HasKey(ta => new { ta.LoginJudge, ta.LoginOrganization });

            builder
                .HasOne(org => org.Organization)
                .WithMany(o => o.OrganizationJudge)
                .HasForeignKey(fk => fk.LoginOrganization)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(j => j.Judge)
                .WithMany(jo => jo.OrganizationJudge)
                .HasForeignKey(j => j.LoginJudge)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}