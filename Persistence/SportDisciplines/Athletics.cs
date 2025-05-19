using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DB.SportHive.MongoDb
{
    public class AthleticsMatch : MatchEvents
    {
        public AthleticsMatch(ExtremeIndividualInfo info)
        {
            movesAthletis = new List<AthleticsMoves>();
            NameDesipline = info.NameDesipline;
            foreach (var entity in info.playersName)
            {
                var athlete = new AthleticsMoves
                {
                    FullNamePlayer = entity.FullNamePlayer,
                    login = entity.loginPlayer
                };
                movesAthletis.Add(athlete);
            }
        }
        public List<AthleticsMoves> movesAthletis { get; set; } = null!;
        public string NameDesipline { get; set; } = null!;
    }
}