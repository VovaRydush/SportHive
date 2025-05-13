namespace DB.SportHive.MongoDb
{
    public class AthleticsMatch
    {
        public long idMatch { get; set; }
        public List<AthleticsMoves> movesAthletis { get; set; } = null!;
    }
}