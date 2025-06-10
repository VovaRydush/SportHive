
namespace DB.SportHive.Domain
{
    public class Matchs
    {
        public string NameEvent { get; set; } = null!;
        public long IdEvent { get; set; }
        public List<string> Entitys { get; set; } = null!;
        public string? AddInformation { get; set; } = null!;
        public int tour { get; set; }
        public int? Group { get; set; }
        public string NameSport { get; set; } = null!;
        public string loginJudge { get; set; } = null!;
        public string typeSport { get; set; } = null!; // типу індивідуальний чи командний і тд
        public int? ratingElo1 { get; set; }
        public int? ratingElo2 { get; set; }
        public bool Rating { get; set; }
        public string system { get; set; } = null!;
    }
}