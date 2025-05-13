namespace DB.SportHive.MongoDb
{
    public class Rowing
    {
        public long idMatch { get; set; }
        public int Tour{get;set;}
        public string BoatType { get; set; } = null!; 
        public string Discipline { get; set; } = null!; 
        public List<RowingRace> rowingAtheletes {get;set;} = null!;
    }
}