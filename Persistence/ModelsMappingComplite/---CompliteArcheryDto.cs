namespace DB.SportHive.Domain
{
    public class CompliteArcheryDto
    {
        public string CompetitionType { get; set; } = null!;
        public string BowType { get; set; } = null!;
        public float Distance { get; set; }
        public CompliteMatchInfo MatchInfo { get; set; } = null!;
    }
}