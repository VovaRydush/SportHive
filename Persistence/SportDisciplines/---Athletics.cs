using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class AthleticsMatch : MatchEvents
    {
        public AthleticsMatch(){}
        public AthleticsMatch(ExtremeIndividualInfo info)
        {
            idEvent = info.idEvent;
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
                composition.Add(entity.EntityName);
            }
        }
        public List<AthleticsMoves> movesAthletis { get; set; } = new();
        public string NameDesipline { get; set; } = null!;
    }
}