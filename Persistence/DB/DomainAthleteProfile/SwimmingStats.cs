namespace DB.SportHive.Domain
{
    public class SwimmingStats : ISportStats
    {
        public List<SwimmingDiscipline> Disciplines { get; set; } = new();
    }

    public class SwimmingDiscipline
    {
        public string StrokeType { get; set; } = null!;       // "Кроль", "Батерфляй", "Брас"
        public int DistanceMeters { get; set; }       // Наприклад: 50, 100, 200
        public string BestTime { get; set; } = null!;     // hh:mm:ss.ms або як рядок
    }
}
