using DB.SportHive.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SportHive.DbConfiguration
{
    public class AthleteConfiguration : IEntityTypeConfiguration<Athlete>
    {
        public void Configure(EntityTypeBuilder<Athlete> builder)
        {
            builder.ToTable("Athlete");

            builder.HasKey(a => a.login);

            builder.Property(a => a.login)
                .HasColumnName("Login")
                .HasMaxLength(40);

            builder.Property(a => a.FirsName)
                .HasColumnName("FirsName")
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(a => a.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(a => a.DataBirth)
                .HasColumnName("DataBirth");

            builder.Property(a => a.TypeSport)
                .HasColumnName("TypeSport")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Athlete>(a => a.login)
                .HasPrincipalKey<User>(u => u.login)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
