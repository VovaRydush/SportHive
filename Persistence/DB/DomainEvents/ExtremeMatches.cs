using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("ExtremeMatch")]
    public class ExtremeMatch
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdExtremeMatches { get; set; }

        [Required]
        public long IdEvent { get; set; }

        public Event Event { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime DataMatch { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan TimeMatch { get; set; }

        [Required]
        public Location Location { get; set; } = null!;
        public string LocationName { get; set; } = null!;
        public StatusMatch StatusMatch { get; set; }

        [Column("Tour")]
        public int Tour { get; set; }

        [Column("AddInformation")]
        [MaxLength(300)]
        public string AddInformation { get; set; } = null!;
        public List<EMatchesAthlete>? MatchesAthletes { get; set; }
    }
}
