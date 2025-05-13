namespace DB.SportHive.MongoDb
{
    public class DistanceRunningMatch
    {
        public long idMatch { get;}
        public List<DistanceRunning> runningAthlets{get;set;} = null!;
    }
}