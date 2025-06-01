namespace DB.SportHive.Domain
{
    public class BasketballStats : ISportStats
    {
        public int Matches { get; set; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
        public int Blocks { get; set; }
        public double ThreePointPercentage { get; set; }
        public double FreeThrowPercentage { get; set; }
        public bool WasMVP { get; set; }
    }
}
