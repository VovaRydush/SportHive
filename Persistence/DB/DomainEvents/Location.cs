using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("Location")]
    public class Location
    {
        [Key]
        [Required]
        public string LocationName { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string? LocationNameEnd { get; set; } = null!;
        public decimal? LatitudeEnd { get; set; }
        public decimal? LongitudeEnd { get; set; }
        public long idMatch{ get; set; }
    }
}
