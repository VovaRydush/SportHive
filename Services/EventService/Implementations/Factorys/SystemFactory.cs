using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SystemFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SystemFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public ICompetitionSystem Create(string system)
        {
            var scope = _scopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            return system switch
            {
                "PlayOff" => provider.GetRequiredService<PlayOffSystem>(),
                "DoubleElimination" => provider.GetRequiredService<DoubleEliminationSystem>(),
                "Group" => provider.GetRequiredService<GroupSystem>(),
                "Knockout" => provider.GetRequiredService<KnockoutSystem>(),
                "Olympic" => provider.GetRequiredService<OlympicSystem>(),
                "RoundRobin" => provider.GetRequiredService<RoundRobinSystem>(),
                "Swiss" => provider.GetRequiredService<SwissSystem>(),
                _ => throw new NotImplementedException($"Unknown system: {system}")
            };
        }
    }


}
