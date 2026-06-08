using DB.SportHive.Persistence;
using Events.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace SportHive.Implementations.Background
{
    public class EventInvitationDeadlineWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventInvitationDeadlineWorker> _logger;

        public EventInvitationDeadlineWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<EventInvitationDeadlineWorker> logger)
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
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    await EventInvitationEndpoints.CheckExpiredBatches(db, config);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking expired event invitation batches");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
