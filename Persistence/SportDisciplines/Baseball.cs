using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Baseball : MatchEvents
    {
        public Baseball(){}
        public Baseball(ExtremeIndividualInfo info)
        {
            fouls = new List<PlayerFouls>();
            Tour = info.tour;
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
        public int Tour { get; set; }
        public List<BaseballEvent> baseballEvents{get;set;} = new();
        public List<PointBasketball> pointBasketball{ get; set; } = new();
        public List<PlayerFouls> fouls { get; set; } = new();
    }
}