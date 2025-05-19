using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SystemFactory
    {
        private readonly IServiceProvider _provider;

        public SystemFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ICompetitionSystem Create(string system)
        {
            return system switch
            {
                "PlayOff" => _provider.GetRequiredService<PlayOffSystem>(),
                "DoubleElimination" => _provider.GetRequiredService<DoubleEliminationSystem>(),
                "Group" => _provider.GetRequiredService<GroupSystem>(),
                "Knockout" => _provider.GetRequiredService<KnockoutSystem>(),
                "Mixsed" => _provider.GetRequiredService<MixsedSystem>(),
                "Olympic" => _provider.GetRequiredService<OlympicSystem>(),
                "RoundRobin" => _provider.GetRequiredService<RoundRobinSystem>(),
                "Swiss" => _provider.GetRequiredService<SwissSystem>(),
                _ => throw new NotImplementedException($"Unknown system: {system}")
            };
        }
    }

}
