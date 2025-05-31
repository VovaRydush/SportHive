namespace DB.SportHive.Domain
{ 
    public class ArcheryStats
{
    public int Competitions { get; set; }
    public int MaxPointsPerRound { get; set; }
    public double AveragePointsPerRound { get; set; }
    public int Bullseyes { get; set; }            // Пряме влучання в центр
    public string DistanceType { get; set; } = null!;     // Напр: "30m", "70m", "Olympic"
    public List<string> Medals { get; set; } = new();
}

}