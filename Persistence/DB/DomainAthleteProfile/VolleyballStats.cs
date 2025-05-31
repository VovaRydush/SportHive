namespace DB.SportHive.Domain
{
    public class VolleyballStats
    {
        public int MatchesPlayed { get; set; }
        public int Aces { get; set; }              // Подачі напряму в очко
        public int Blocks { get; set; }            // Блоки
        public int AttackPoints { get; set; }      // Очки в атаці
        public int Errors { get; set; }            // Помилки
        public int Wins { get; set; }
        public string Position { get; set; } = null!;     // Наприклад: "Ліберо", "Діагональний"
    }
}
