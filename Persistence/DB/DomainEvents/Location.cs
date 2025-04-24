using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("Location")]
    public class Location
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string LocationName { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
