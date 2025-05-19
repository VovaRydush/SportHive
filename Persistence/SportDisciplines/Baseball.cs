using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Baseball : MatchEvents
    {
        public Baseball(ExtremeIndividualInfo info)
        {
            playerFouls = new List<PlayerFouls>();
            Tour = info.tour;
            foreach (var entity in info.playersName)
            {
                var athlete = new PlayerFouls
                {
                    FullNamePlayer = entity.FullNamePlayer,
                    loginPlayer = entity.loginPlayer
                };
                playerFouls.Add(athlete);
            }
        }
        public int Tour { get; set; }
        public List<BaseballEvent> baseballEvents{get;set;} = null!;
        public List<PlayerFouls> playerFouls {get;set;} = null!;
    }
}