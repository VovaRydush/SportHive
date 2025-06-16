namespace DB.SportHive.Domain
{
    public class ArcheryPlayerEventsResult
    {
        public string TypeSport { get; set; } = "Стрільба з лука";
        public List<ArcheryPlayerEventRes> Events { get; set; } = new();
    }

    public class ArcheryPlayerEventRes
    {
        public string NameEvent { get; set; } = null!;
        public string EventPhoto { get; set; } = null!;
        public string TeamName { get; set; } = null!;
        public string PlayerName { get; set; } = null!;
    }
}