
namespace DB.SportHive.Domain
{
    public class Matchs
    {
        public string NameEvent { get; set; } = null!;
        public List<string> Entitys { get; set; } = null!;
        public string? AddInformation { get; set; } = null!;
        public int tour { get; set; }
        public string NameSport { get; set; } = null!;
        public string typeSport { get; set; } = null!; // типу індивідуальний чи командний і тд
        public bool? Raiting { get; set; }
        public string system { get; set; } = null!;
    }
}