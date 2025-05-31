namespace DB.SportHive.Domain
{ 
    public class CyclingStats
{
    public int Races { get; set; }
    public double BestTime { get; set; }            // у секундах
    public string BestDistance { get; set; } = null!;       // "10km", "40km", "Time Trial"
    public double AverageSpeedKmH { get; set; }     // середня швидкість
    public int StageWins { get; set; }
    public List<string> RaceTypes { get; set; } = null!;     // "Road", "Track", "MTB", "BMX"
    public List<string> Medals { get; set; } = new();
}

}