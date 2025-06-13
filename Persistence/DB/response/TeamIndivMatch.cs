namespace DB.SportHive.Domain
{
    public class TeamIndivMatch
    {
        public List<TeamIndivMatchRes> Matches { get; set; } = null!;
    }
    public class TeamIndivMatchRes
    {
        public string NameEntity1 { get; set; } = null!;
        public string photoFirstEntity { get; set; } = null!;
        public string NameEntity2 { get; set; } = null!;
        public string photoSecondEntity { get; set; } = null!;
        public string statusMatch { get; set; } = null!;
    }
}