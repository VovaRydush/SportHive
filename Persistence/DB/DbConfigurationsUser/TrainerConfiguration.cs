using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.ToTable("Trainer");

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

            builder
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Trainer>(a => a.login)
                .HasPrincipalKey<User>(u => u.login)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
