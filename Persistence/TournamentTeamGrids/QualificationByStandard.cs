namespace DB.SportHive.Domain
{
    public class QualificationGrid
    {
        public long idEvent { get; set; }
        public int tour { get; set; }
        public string NameEntity { get; set; } = null!;
        public string Category { get; set; } = null!;
        public bool isNext { get; set; }
    }
}