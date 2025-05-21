using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveMatchFactory
    {
         private readonly IServiceScopeFactory _scopeFactory;

        public SaveMatchFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public ISaveMatchInfo Create(string matchType)
        {
             var scope = _scopeFactory.CreateScope();
             var _provider = scope.ServiceProvider;

            return matchType switch
            {
                "SaveTeamMatch" => _provider.GetRequiredService<SaveTeamMatch>(),
                "IndividualMatch" => _provider.GetRequiredService<SaveIndividualMatch>(),
                "ExtremeMatch" => _provider.GetRequiredService<SaveExtremeMatch>(),
                _ => throw new NotFoundException($"Unknown match type: {matchType}")
            };
        }
    }





}
