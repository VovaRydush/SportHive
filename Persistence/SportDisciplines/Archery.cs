using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Archery : MatchEvents
    {
        public Archery(){}
        public Archery(ExtremeIndividualInfo info)
        {
            fouls = new List<PlayerFouls>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new PlayerFouls
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                fouls.Add(athlete);
            }
        }
        public string CompetitionType { get; set; } = null!;
        public List<PlayerFouls> fouls { get; set; }  = new();
        public string BowType { get; set; } = null!;
        public float Distance { get; set; }
    }
}
