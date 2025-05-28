using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class WeightliftingMatch : MatchEvents
    {
        public WeightliftingMatch(){}
        public WeightliftingMatch(ExtremeIndividualInfo info)
        {
            liftingsAthlete = new List<Weightlifting>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new Weightlifting
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                liftingsAthlete.Add(athlete);
            }
        }
        public List<PlayerFouls> fouls { get; set; } = new();
        public List<Weightlifting> liftingsAthlete { get; set; } = new();
    }
}