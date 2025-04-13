using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class OrganizationJudgeConfiguration : IEntityTypeConfiguration<OrganizationJudge>
    {
        public void Configure(EntityTypeBuilder<OrganizationJudge> builder)
        {
            builder.HasKey(ta => new { ta.IdJudge, ta.IdOrganization });

            builder
                .HasOne(org => org.Organization)
                .WithMany(o => o.OrganizationJudge)
                .HasForeignKey(fk => fk.IdOrganization);
            
            builder
                .HasOne(j => j.Judge)
                .WithMany(jo => jo.OrganizationJudge)
                .HasForeignKey(j => j.IdJudge);
        }
    }
}