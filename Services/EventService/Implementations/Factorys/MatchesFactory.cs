using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveMatchFactory
    {
        private readonly IServiceProvider _provider;

        public SaveMatchFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ISaveMatchInfo Create(string matchType)
        {
            return matchType switch
            {
                "SaveTeamMatch" => _provider.GetRequiredService<SaveTeamMatch>(),
                "IndividualMatch" => _provider.GetRequiredService<SaveIndividualMatch>(),
                "TeamMatch" => _provider.GetRequiredService<SaveTeamMatch>(),
                _ => throw new NotFoundException($"Unknown match type: {matchType}")
            };
        }
    }



}
