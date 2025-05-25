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
            foreach (var entity in info.entitysName)
            {
                var athlete = new AthleticsMoves
                {
                    FullNamePlayer = entity.EntityName,
                    login = entity.loginPlayer ?? ""
                };
                movesAthletis.Add(athlete);
            }
        }
        public List<AthleticsMoves> movesAthletis { get; set; } = new();
        public string NameDesipline { get; set; } = null!;
    }
}