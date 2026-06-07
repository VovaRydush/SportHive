namespace NotificationService;

using Confluent.Kafka;
using DB.SportHive.Domain;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

public class ConsumerEmail : BackgroundService
{
    private readonly ILogger<ConsumerEmail> _logger;
    private readonly IDatabase _database;
    private readonly ConsumerConfig config;
    private readonly IConsumer<Null, string> consumer;
    private readonly IConfiguration _configuration;

    public ConsumerEmail(
        ILogger<ConsumerEmail> logger,
        IConnectionMultiplexer database,
        IConfiguration configuration)
    {
        _database = database.GetDatabase();
        _logger = logger;
        _configuration = configuration;

        config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9093",
            GroupId = "email-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe("user_email");
    }

    public async Task SendEmail(EnhancedEmailMessageDto message)
    {
        try
        {
            var smtpHost = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_configuration["Smtp:Port"], out var parsedPort) ? parsedPort : 587;
            var smtpUser = _configuration["Smtp:Username"] ?? "vadimrudis7@gmail.com";
            var smtpPassword = _configuration["Smtp:Password"] ?? "qtfo apob lfjd nqfp";
            var from = string.IsNullOrWhiteSpace(message.From)
                ? (_configuration["Smtp:From"] ?? smtpUser)
                : message.From;

            using var smtp = new SmtpClient(smtpHost)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true,
                Port = smtpPort
            };

            var subject = string.IsNullOrWhiteSpace(message.Subject)
                ? "SportHive"
                : message.Subject;

            string body;

            if (!string.IsNullOrWhiteSpace(message.Body))
            {
                body = message.IsBodyHtml
                    ? message.Body
                    : ToHtml(message.Body);
            }
            else
            {
                // Backward compatibility with old registration/recovery emails:
                // old AuthService sends only To/From/Subject, so we generate confirmation code as before.
                var random = new Random();
                var code = random.Next(100000, 1000000).ToString();
                _ = _database.StringSetAsync(message.To, code, TimeSpan.FromMinutes(10));
                body = HTMLTemplate.getHTMLPage(code, subject);
            }

            var mail = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var recipient in message.To.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(recipient.Trim());
            }

            await smtp.SendMailAsync(mail);
            _logger.LogInformation("Email sent to {Email} with subject {Subject}", message.To, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", message.To);
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
                    var message = JsonSerializer.Deserialize<EnhancedEmailMessageDto>(
                        cr.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (message is null || string.IsNullOrWhiteSpace(message.To))
                    {
                        _logger.LogWarning("Invalid email message from Kafka: {Message}", cr.Message.Value);
                        consumer.Commit(cr);
                        continue;
                    }

                    await SendEmail(message);
                    consumer.Commit(cr);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka email message: {Message}", cr.Message.Value);
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

    private static string ToHtml(string text)
    {
        return "<div style=\"font-family:Arial,sans-serif;line-height:1.5\">" +
               WebUtility.HtmlEncode(text).Replace("\n", "<br/>") +
               "</div>";
    }
}

public sealed class EnhancedEmailMessageDto
{
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public string Subject { get; set; } = "";
    public string? Body { get; set; }
    public bool IsBodyHtml { get; set; } = true;
    public string? Kind { get; set; }
}
