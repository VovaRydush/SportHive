using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Swimming : MatchEvents
    {
        public Swimming(){}
        public Swimming(ExtremeIndividualInfo info)
        {
            idEvent = info.idEvent;
            var athleteSwimming = new List<AthleteSwimming>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new AthleteSwimming
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                athleteSwimming.Add(athlete);
                composition.Add(athlete.loginPlayer);
            }
        }
        public List<PlayerFouls> fouls { get; set; } = new();
        public List<AthleteSwimming> athleteSwimming { get; set; } = new();
        
    }
}