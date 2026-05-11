using System.Text;
using Confluent.Kafka;
using SportHive.SaveServices.Interfaces;

public class KafkaWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaWorker> _logger;


    public KafkaWorker(ILogger<KafkaWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "user-save-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }



    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(new List<string> { "user_regist", "user-photo", "user-judge", "user-trainer", "user-athlete", "user-organization" });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var headers = consumeResult?.Message?.Headers;
                var header = headers?.FirstOrDefault(h => h.Key == "save-only-data");

                if (consumeResult != null && header != null && Encoding.UTF8.GetString(header.GetValueBytes()) == "save-only-data")
                {
                    using var scope = _scopeFactory.CreateScope();
                    var saveService = scope.ServiceProvider.GetRequiredService<ISaveDataDb>();
                    await saveService.SaveDataUser(consumeResult.Message.Value, consumeResult.Topic);
                    _consumer.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka message");
            }
        }
    }

}
