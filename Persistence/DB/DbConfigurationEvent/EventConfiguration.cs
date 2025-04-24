using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DB.SportHive.Domain;

namespace SportHive.DbConfiguration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder
                .HasIndex(e => e.IdEvent)
                .IsUnique();

            builder
                .Property(u => u.systems)
                .HasConversion(
                    v => v.ToString(),
                    v => (SelectionSystems)Enum.Parse(typeof(SelectionSystems), v));
        }
    }
}