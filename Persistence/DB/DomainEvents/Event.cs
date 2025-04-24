using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("Event")]
    public class Event
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdEvent { get; set; }

        [Required]
        [Column("NameEvent")]
        [MaxLength(100)]
        public string NameEvent { get; set; } = null!;

        [Required]
        public SelectionSystems systems {get;set;}

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataStart {get;set;}

        [DataType(DataType.Date)]
        public DateTime DataEnd {get;set;}

        [MaxLength(300)]
        public string description{get;set;} = null!;

    }
    public enum SelectionSystems
    {
        RoundRobin = 1,
        PlayOff = 2,
        MixedSystem = 3,
        SwissSystem = 4,
        QualificationByStandards = 5,
        GroupStage = 6,
        Final = 7,
        OlympicSystem = 8,
        KnockoutSystem = 9,
        DoubleElimination = 10
    }
}
