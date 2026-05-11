namespace NotificationService;
using Confluent.Kafka;
using DB.SportHive.Domain;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading;
public class ConsumerEmail : BackgroundService
{
    private readonly ILogger<ConsumerEmail> _logger;
    private readonly IDatabase _database;
    private ConsumerConfig config;
    private IConsumer<Null, string> consumer;

    public ConsumerEmail(ILogger<ConsumerEmail> logger,IConnectionMultiplexer database)
    {
        _database = database.GetDatabase();
        _logger = logger;
        config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "email-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe("user_email");
    }

    public async Task SendEmail(EmailMessageDto message)
    {
        try
        {
            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new NetworkCredential("vadimrudis7@gmail.com", "qtfo apob lfjd nqfp"),
                EnableSsl = true,
                Port = 587
            };
            Random random = new Random();
            string code = random.Next(100000, 1000000).ToString();
           
            _ = _database.StringSetAsync(message.To, code,TimeSpan.FromMinutes(10));

            var mail = new MailMessage
            {
                From = new MailAddress(message.From),
                Subject = message.Subject,
                Body = HTMLTemplate.getHTMLPage(code, message.Subject),
                IsBodyHtml = true 
            };

            
            foreach (var recipient in message.To.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(recipient.Trim());
            }

            await smtp.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(stoppingToken);
                try
                {
                    EmailMessageDto message = JsonSerializer.Deserialize<EmailMessageDto>(cr.Message.Value);
                    await SendEmail(message);
                    consumer.Commit(cr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error deserializing message: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer has been cancelled.");
        }
        finally
        {
            consumer.Close();
        }
    }
}

