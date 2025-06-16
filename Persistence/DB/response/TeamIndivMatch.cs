namespace DB.SportHive.Domain
{
    public class TeamIndivMatch
    {
        public string TypeSport { get; set; } = null!;
        public List<TeamIndivMatchRes> Matches { get; set; } = null!;
    }
    public class TeamIndivMatchRes
    {
        public string firstTeamScore { get; set; } = null!;
        public string secondTeamScore { get; set;} = null!;
        public string NameEntity1 { get; set; } = null!;
        public string photoFirstEntity { get; set; } = null!;
        public string NameEntity2 { get; set; } = null!;
        public string photoSecondEntity { get; set; } = null!;
        public string statusMatch { get; set; } = null!;
    }
}