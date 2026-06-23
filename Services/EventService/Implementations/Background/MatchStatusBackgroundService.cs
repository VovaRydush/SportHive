using DB.SportHive.Persistence;

namespace SportHive.Implementations.Background
{
    public sealed class MatchStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MatchStatusBackgroundService> _logger;

        public MatchStatusBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<MatchStatusBackgroundService> logger)
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
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var changed = await MatchStatusNormalizer.NormalizeAsync(db, stoppingToken);

                    if (changed > 0)
                        _logger.LogInformation("Match statuses normalized. Changed rows: {Changed}", changed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Match status normalization failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
