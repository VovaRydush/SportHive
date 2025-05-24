using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("TeamMatch")]
    public class TeamMatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdTeamMatch { get; set; }

        [Required]
        public long IdEvent { get; set; }
        public Event Event { get; set; } = null!;

        public StatusMatch StatusMatch { get; set; }

        [Required]
        public string NameFirstTeam { get; set; } = null!;
        public Team FirstTeam { get; set; } = null!;

        [Required]
        public string NameSecondTeam { get; set; } = null!;
        public Team SecondTeam { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DataMatch { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? TimeMatch { get; set; }

        public Location? Location { get; set; } = null!;
        public string? LocationName { get; set; } = null!;

        [Column("Tour")]
        public int Tour { get; set; }
        public int? Group{ get; set; }

        [Column("AddInformation")]
        [MaxLength(300)]
        public string? AddInformation { get; set; } = null!;
    }
}
