using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class WeightliftingMatch : MatchEvents
    {
        public WeightliftingMatch(ExtremeIndividualInfo info)
        {
            liftingsAthlete = new List<Weightlifting>();
            foreach (var entity in info.playersName)
            {
                var athlete = new Weightlifting
                {
                    FullNamePlayer = entity.FullNamePlayer,
                    loginPlayer = entity.loginPlayer
                };
                liftingsAthlete.Add(athlete);
            }
        }
        public List<Weightlifting> liftingsAthlete { get; set; } = null!;
    }
}