using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("IndividualMatch")]
    public class IndividualMatch
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdIndividualMatch { get; set; }

        [Required]
        public long IdEvent { get; set; }
        public string? loginJudge { get; set; } 
        public Judge? Judge { get; set; }

        public Event Event { get; set; } = null!;
        public StatusMatch StatusMatch { get; set; }

        [Required]
        public string loginFirstAthlete { get; set; } = null!;

        public Athlete FirstAthlete { get; set; } = null!;

        [Required]
        public string loginSecondAthlete { get; set; } = null!;
        public Athlete SecondAthlete { get; set; } = null!;


        [DataType(DataType.Date)]
        public DateTime? DataMatch { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? TimeMatch { get; set; }

        [Required]
        public Location? Location { get; set; } = null!;
        public string? LocationName { get; set; } = null!;

        [Column("Tour")]
        public int Tour { get; set; }

        [Column("AddInformation")]
        [MaxLength(300)]
        public string? AddInformation { get; set; } = null!;
    }
}
