
namespace DB.SportHive.Domain
{
    public class MatchsAbstractionDto
    {
        public string NameEvent { get; set; } = null!;
        public List<string>? Entitys { get; set; } = null!;
        public string AddInformation { get; set; } = null!;
        public int tour { get; set; }
        public string type { get; set; } = null!;
        public string system { get; set; } = null!;
    }
}