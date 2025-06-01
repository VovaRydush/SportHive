using DB.SportHive.Domain;
using SportHive.Exceptions;


namespace SportHive.Implementations
{
    public class UserProfileFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public UserProfileFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public object CreateUserProfile(string sportType)
        {

            var scope = _scopeFactory.CreateScope();
            var _provider = scope.ServiceProvider;

            return sportType switch
            {
                "AmericanFootball" => _provider.GetRequiredService<AmericanFootballStats>(),
                "Archery" => _provider.GetRequiredService<ArcheryStats>(),
                "Basketball" => _provider.GetRequiredService<BasketballStats>(),
                "Chess" => _provider.GetRequiredService<CheckersChessStats>(),
                "Checkers" => _provider.GetRequiredService<CheckersChessStats>(),
                "Box" => _provider.GetRequiredService<BoxingStats>(),
                "Cycling" => _provider.GetRequiredService<CyclingStats>(),
                "Football" => _provider.GetRequiredService<FootballStats>(),
                "IceHockey" => _provider.GetRequiredService<IceHockeyStats>(),
                "Powerlifting" => _provider.GetRequiredService<PowerliftingStats>(),
                "TableTenis" => _provider.GetRequiredService<RacketSportsStats>(),
                "Tenis" => _provider.GetRequiredService<RacketSportsStats>(),
                "Badminton" => _provider.GetRequiredService<RacketSportsStats>(),
                "Rowing" => _provider.GetRequiredService<RowingStats>(),
                "Rugby" => _provider.GetRequiredService<RugbyStats>(),
                "Struggle" => _provider.GetRequiredService<StruggleStats>(),
                "Swimming" => _provider.GetRequiredService<SwimmingStats>(),
                "VolleyballStats" => _provider.GetRequiredService<VolleyballStats>(),
                "Weightlifting" => _provider.GetRequiredService<WeightliftingStats>(),
                _ => throw new NotFoundException($"Unknown match type: {sportType}")
            };
        }
    }
}