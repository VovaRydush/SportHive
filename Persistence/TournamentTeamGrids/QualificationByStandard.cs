namespace DB.SportHive.Domain
{
    public class QualificationGrid : TeamIndivGrid
    {
        public string NameEntity { get; set; } = null!;
        public string Category { get; set; } = null!;
        public bool isNext { get; set; }
    }
}