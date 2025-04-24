using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class IndividualMatchConfig : IEntityTypeConfiguration<IndividualMatch>
    {
        public void Configure(EntityTypeBuilder<IndividualMatch> builder)
        {
            builder.HasKey(im => im.IdIndividualMatch);
            
            builder.HasOne(im => im.Event)
                   .WithMany()
                   .HasForeignKey(im => im.IdEvent)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(im => im.FirstAthlete)
                   .WithMany()
                   .HasForeignKey(im => im.loginFirstAthlete)
                   .HasPrincipalKey(a => a.login)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(im => im.SecondAthlete)
                   .WithMany()
                   .HasForeignKey(im => im.loginSecondAthlete)
                   .HasPrincipalKey(a => a.login)
                   .OnDelete(DeleteBehavior.Restrict);
            
             builder.HasOne(l => l.Location)             
                   .WithMany()                      
                   .HasForeignKey(tm => tm.LocationName)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}