using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Swimming : MatchEvents
    {
        public Swimming(ExtremeIndividualInfo info)
        {
            athleteSwimming = new List<AthleteSwimming>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new AthleteSwimming
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                athleteSwimming.Add(athlete);
            }
        }
        public List<AthleteSwimming> athleteSwimming { get; set; } = null!;
        
    }
}