namespace DB.SportHive.Domain
{
    public class TeamIndivGrid
    {
        public long idEvent { get; set; }
        public long idMatch { get; set; }
        public int tour { get; set; }
        public string NameFirstEntity { get; set; } = null!;
        public string NameSecondEntity { get; set; } = null!;
        public int totalScoreEntity1 { get; set; }
        public int totalScoreEntity2 { get; set; }
        public bool? falloutLoser { get; set; }
        public bool played { get; set; } 
    }
}