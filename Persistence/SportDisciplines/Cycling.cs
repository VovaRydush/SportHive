using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Cycling : MatchEvents
    {
        public Cycling(ExtremeIndividualInfo info)
        {
            atheltesMoves = new List<CyclingRace>();
            Tour = info.tour;
            foreach (var entity in info.entitysName)
            {
                var athlete = new CyclingRace
                {
                    AthleteName = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                atheltesMoves.Add(athlete);
            }
        }
        public int Tour { get; set; }
        public string RaceType { get; set; } = null!;
        public List<CyclingRace> atheltesMoves {get;set;} = new();
        public string NameWinner {get;set;} = null!;
    }
}