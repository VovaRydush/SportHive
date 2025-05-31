namespace DB.SportHive.Domain
{
    public class CoachAchievement
    {
        public string Title { get; set; } = null!;             // "Чемпіонат України U16"
        public string TeamName { get; set; } = null!;
        public string Result { get; set; } = null!;            // "1 місце", "Фіналіст"
        public DateTime Date { get; set; }
    }
}
