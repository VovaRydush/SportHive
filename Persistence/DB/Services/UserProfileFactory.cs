using DB.SportHive.Domain;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;


namespace SportHive.Implementations
{
    public class UserProfileFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public UserProfileFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public SportStats CreateUserProfile(string sportType)
        {
            SportStats stats = sportType switch
            {
                "AmericanFootball" => new AmericanFootballStats(),
                "Archery" => new ArcheryStats(),
                "Basketball" => new BasketballStats(),
                "Chess" => new CheckersChessStats(),
                "Checkers" => new CheckersChessStats(),
                "Box" => new BoxingStats(),
                "Cycling" => new CyclingStats(),
                "Football" => new FootballStats(),
                "IceHockey" => new IceHockeyStats(),
                "Powerlifting" => new PowerliftingStats(),
                "TableTenis" => new RacketSportsStats(),
                "Tenis" => new RacketSportsStats(),
                "Badminton" => new RacketSportsStats(),
                "Rowing" => new RowingStats(),
                "Rugby" => new RugbyStats(),
                "Struggle" => new StruggleStats(),
                "Swimming" => new SwimmingStats(),
                "VolleyballStats" => new VolleyballStats(),
                "Weightlifting" => new WeightliftingStats(),
                _ => throw new ArgumentException($"Unknown sport type: {sportType}", nameof(sportType))
            };
            var document = stats.ToBsonDocument();
            document["_t"] = stats.GetType().Name; 

            return BsonSerializer.Deserialize<SportStats>(document);
        }
    }
}