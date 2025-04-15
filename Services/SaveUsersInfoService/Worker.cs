using System.Net.Http.Json;
using System.Text.Json;
using Confluent.Kafka;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;

public class KafkaWorker : BackgroundService
{
    private readonly ILogger<KafkaWorker> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;

    public KafkaWorker(ILogger<KafkaWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9093",
            GroupId = "user-save-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    private async Task ProcessMessage(string jsonObj, string topic)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        switch (topic)
        {
            case "user_regist":
                var user = JsonSerializer.Deserialize<User>(jsonObj);
                dbContext.Users.Add(user);
                break;
            
            case "user-athlete":
              var athlete = JsonSerializer.Deserialize<Athlete>(jsonObj);
              dbContext.Athletes.Add(athlete);
                break;

            case "user-trainer":
              var trainer = JsonSerializer.Deserialize<Trainer>(jsonObj);
              dbContext.Trainers.Add(trainer);
                break;

            case "user-judge":
            var judge = JsonSerializer.Deserialize<Judge>(jsonObj);
            dbContext.Judges.Add(judge);
                break;

            case "user-photo":
            var userphoto = JsonSerializer.Deserialize<UserPhoto>(jsonObj);
            dbContext.UserPhotos.Add(userphoto);
                break;
            
            case "user-organization":
            var organization = JsonSerializer.Deserialize<Organization>(jsonObj);
            dbContext.Organizations.Add(organization);
                break;

            default:
                _logger.LogWarning($"Unknown topic: {topic}");
                return;
        }

        await dbContext.SaveChangesAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(new List<string> { "user_regist","user-photo","user-judge","user-trainer","user-athlete","user-organization" });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                if (consumeResult != null)
                {
                    await ProcessMessage(consumeResult.Message.Value, consumeResult.Topic);
                    _consumer.Commit();
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Error while consuming: {e.Error.Reason}");
            }
        }

        _consumer.Close();
    }
}
