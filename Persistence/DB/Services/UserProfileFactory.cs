using DB.SportHive.Domain;
using Microsoft.Extensions.DependencyInjection;


namespace SportHive.Implementations
{
    public class UserProfileFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public UserProfileFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public ISportStats CreateUserProfile(string sportType)
        {
            return sportType switch
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
                "Weightlifting" => new WeightliftingStats()
            };
        }
    }
}