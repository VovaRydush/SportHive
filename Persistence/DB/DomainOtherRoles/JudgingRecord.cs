namespace DB.SportHive.Domain
{
    public class JudgingRecord
    {
        public string EventId { get; set; } = null!;
        public string EventName { get; set; } = null!;
        public string SportType { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Role { get; set; } = null!; // Наприклад: "Головний суддя", "Асистент"
    }
}
