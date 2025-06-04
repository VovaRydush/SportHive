using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class AthleteProfileFactory
    {
        private readonly IServiceProvider _provider;

        public AthleteProfileFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IProfileInfo CreateTypeSport(string typeSport)
        {
            return typeSport switch
            {
                "AmericanFootball" => _provider.GetRequiredService<AmericanFootballInfo>(),
                "Archery" => _provider.GetRequiredService<ArcheryInfo>(),
                "Basketball" => _provider.GetRequiredService<BasketballInfo>(),
                "Box" => _provider.GetRequiredService<BoxInfo>(),
                "Checkers" => _provider.GetRequiredService<CheckersChessInfo>(),
                "Chess" => _provider.GetRequiredService<CheckersChessInfo>(),
                "Cycling" => _provider.GetRequiredService<CyclingInfo>(),
                "Football" => _provider.GetRequiredService<FootballInfo>(),
                "IceHockey"=> _provider.GetRequiredService<HockeyInfo>(),
                "Powerlifting" => _provider.GetRequiredService<PowerliftingInfo>(),
                "RacketSports" => _provider.GetRequiredService<RacketSportsInfo>(),
                "Rowing" => _provider.GetRequiredService<RowingInfo>(),
                "Rugby" => _provider.GetRequiredService<RugbyInfo>(),
                "Struggle" => _provider.GetRequiredService<StruggleInfo>(),
                "Swimming" => _provider.GetRequiredService<SwimmingInfo>(),
                "Volleyball" => _provider.GetRequiredService<VolleyballInfo>(),
                "Weightlifting" => _provider.GetRequiredService<WeightliftingInfo>(),
                _ => throw new NotImplementedException($"Unknown system: {typeSport}")
            };
        }
    }
}