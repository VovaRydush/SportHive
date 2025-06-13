using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonKnownTypes(
        typeof(FootballStats),
        typeof(StruggleStats),
        typeof(AmericanFootballStats),
        typeof(ArcheryStats),
        typeof(BasketballStats),
        typeof(BoxingStats),
        typeof(CheckersChessStats),
        typeof(CyclingStats),
        typeof(IceHockeyStats),
        typeof(PowerliftingStats),
        typeof(RacketSportsStats),
        typeof(RowingStats),
        typeof(RugbyStats),
        typeof(SwimmingStats),
        typeof(VolleyballStats),
        typeof(WeightliftingStats)
        )]
    [BsonDiscriminator(RootClass = true)]
    public abstract class SportStats
    {
    }
}