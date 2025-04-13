using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class OrganizationTeamConfigration : IEntityTypeConfiguration<OrganizationTeam>
    {
        public void Configure(EntityTypeBuilder<OrganizationTeam> builder)
        {
            builder.HasKey(to => new { to.LoginOrganization, to.NameComand });

            builder
                .HasOne(org => org.Organization)
                .WithMany(o => o.Teams)
                .HasForeignKey(fk => fk.LoginOrganization)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(t => t.Team)
                .WithMany(to => to.OrganizationTeam)
                .HasForeignKey(t => t.NameComand)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}