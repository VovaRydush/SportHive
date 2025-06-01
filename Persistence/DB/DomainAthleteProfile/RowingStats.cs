namespace DB.SportHive.Domain
{
    public class RowingStats : ISportStats
    {
        public int Races { get; set; }
        public string BoatClass { get; set; } = null!;           // "Single", "Double", "Four", "Eight"
        public string Category { get; set; } = null!;             // "Lightweight", "Open", "Mixed"
        public double BestTimeSeconds { get; set; }     // У секундах
        public string BestDistance { get; set; } = null!;         // Наприклад, "2000m"
        public List<string> Medals { get; set; } = new();
    }

}