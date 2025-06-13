using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonKnownTypes(typeof(FootballStats),typeof(StruggleStats))]
    [BsonDiscriminator(RootClass = true)]
    public abstract class SportStats
    {
    }
}