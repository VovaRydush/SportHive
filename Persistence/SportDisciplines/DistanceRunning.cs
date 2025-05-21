using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class DistanceRunningMatch : MatchEvents
    {
        public DistanceRunningMatch(ExtremeIndividualInfo info)
        {
            runningAthlets = new List<DistanceRunning>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new DistanceRunning
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                runningAthlets.Add(athlete);
            }
        }
        public List<DistanceRunning> runningAthlets { get; set; } = null!;
    }
}