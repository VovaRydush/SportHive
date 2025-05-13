namespace DB.SportHive.MongoDb
{
    public class Cycling
    {
        public long idMatch { get; set; }
        public int Tour{get;set;}
        public string RaceType { get; set; } = null!;
        public List<CyclingRace> atheltesMoves {get;set;} = null!;
        public string NameWinner {get;set;} = null!;
    }
}