namespace DB.SportHive.Domain
{
    public class VolleyballStats : ISportStats
    {
        public int Matches { get; set; }
        public int Aces { get; set; }              // Подачі напряму в очко
        public int Blocks { get; set; }            // Блоки
        public int AttackPoints { get; set; }      // Очки в атаці
        public int Errors { get; set; }            // Помилки
        public int Win { get; set; }
        public int Losses { get; set; }
        // Наприклад: "Ліберо", "Діагональний"
    }
}
