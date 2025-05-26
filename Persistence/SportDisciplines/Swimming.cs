using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Swimming : MatchEvents
    {
        public Swimming(ExtremeIndividualInfo info)
        {
            var athleteSwimming = new List<AthleteSwimmingStyle>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new AthleteSwimmingStyle
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                athleteSwimming.Add(athlete);
            }
        }
        public long IdMatch { get; set; }
        public List<PlayerFouls> fouls{ get; set; }
        public List<AthleteSwimmingStyle> athleteSwimmingStyle { get; set; } = new();
        
    }
}