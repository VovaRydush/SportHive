using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Archery : MatchEvents
    {
        public Archery(ExtremeIndividualInfo info)
        {
            playerFouls = new List<PlayerFouls>();
            foreach (var entity in info.entitysName)
            {
                var athlete = new PlayerFouls
                {
                    FullNamePlayer = entity.EntityName,
                    loginPlayer = entity.loginPlayer ?? ""
                };
                playerFouls.Add(athlete);
            }
        }
        public string CompetitionType { get; set; } = null!;
        public List<PlayerFouls> playerFouls { get; set; } = null!;
        public string BowType { get; set; } = null!;
        public float Distance { get; set; }
    }
}
