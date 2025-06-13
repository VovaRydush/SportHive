using DB.SportHive.Domain;
using MongoDB.Bson.Serialization;
namespace SportHive.Implementations
{
    public interface IMongoMappingService
    {
        void RegisterClassMaps();
    }

    public class MongoMappingService : IMongoMappingService
    {
        private bool _isRegistered = false;

        public void RegisterClassMaps()
        {
            if (_isRegistered) return;  // щоб не реєструвати двічі

            BsonClassMap.RegisterClassMap<SportStats>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.AddKnownType(typeof(FootballStats));
                cm.AddKnownType(typeof(AmericanFootballStats));
                cm.AddKnownType(typeof(ArcheryStats));
                cm.AddKnownType(typeof(BasketballStats));
                cm.AddKnownType(typeof(BoxingStats));
                cm.AddKnownType(typeof(CheckersChessStats));
                cm.AddKnownType(typeof(CyclingStats));
                cm.AddKnownType(typeof(IceHockeyStats));
                cm.AddKnownType(typeof(PowerliftingStats));
                cm.AddKnownType(typeof(RacketSportsStats));
                cm.AddKnownType(typeof(RowingStats));
                cm.AddKnownType(typeof(RugbyStats));
                cm.AddKnownType(typeof(StruggleStats));
                cm.AddKnownType(typeof(SwimmingStats));
                cm.AddKnownType(typeof(VolleyballStats));
                cm.AddKnownType(typeof(WeightliftingStats));
            });

            _isRegistered = true;
        }
    }
}