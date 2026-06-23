using SportHive.Services.Interfaces;

namespace SportHive.Implementations.Background
{
    public sealed class MatchCompletionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MatchCompletionBackgroundService> _logger;

        public MatchCompletionBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<MatchCompletionBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IMatchCompletionService>();
                    var changed = await service.NormalizeAllAsync(stoppingToken);

                    if (changed > 0)
                        _logger.LogInformation("Match completion normalized. Changed rows: {Changed}", changed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Match completion normalization failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
