using DB.SportHive.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SportHive.DbConfiguration
{
    public class ExtremeMatchConfiguration : IEntityTypeConfiguration<ExtremeMatch>
    {
        public void Configure(EntityTypeBuilder<ExtremeMatch> builder)
        {

            builder.HasKey(em => em.IdExtremeMatches);

            builder.HasOne(em => em.Event)
                   .WithMany()
                   .HasForeignKey(em => em.IdEvent)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.Location)             
                   .WithMany()                      
                   .HasForeignKey(tm => tm.LocationId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
