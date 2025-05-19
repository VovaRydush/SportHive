using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Cycling : MatchEvents
    {
        public Cycling(ExtremeIndividualInfo info)
        {
            atheltesMoves = new List<CyclingRace>();
            Tour = info.tour;
            foreach (var entity in info.playersName)
            {
                var athlete = new CyclingRace
                {
                    AthleteName = entity.FullNamePlayer,
                    loginPlayer = entity.loginPlayer
                };
                atheltesMoves.Add(athlete);
            }
        }
        public int Tour { get; set; }
        public string RaceType { get; set; } = null!;
        public List<CyclingRace> atheltesMoves {get;set;} = null!;
        public string NameWinner {get;set;} = null!;
    }
}