using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Rowing : MatchEvents
    {
        public Rowing(ExtremeIndividualInfo info)
        {
            rowingAtheletes = new List<RowingRace>();
            Tour = info.tour;
            foreach (var entity in info.entitysName)
            {
                var athlete = new RowingRace
                {
                    AthleteOrTeam = entity.EntityName,
                };
                rowingAtheletes.Add(athlete);
            }
        }
        public int Tour { get; set; }
        public string BoatType { get; set; } = null!; 
        public string Discipline { get; set; } = null!; 
        public List<RowingRace> rowingAtheletes {get;set;} = null!;
    }
}