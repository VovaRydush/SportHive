namespace NotificationService;

using Confluent.Kafka;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;

public class ConsumerEmail : BackgroundService
{
    private readonly ILogger<ConsumerEmail> _logger;
    private readonly IDatabase _database;
    private readonly ConsumerConfig _config;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IConfiguration _configuration;

    public ConsumerEmail(
        ILogger<ConsumerEmail> logger,
        IConnectionMultiplexer database,
        IConfiguration configuration)
    {
        _database = database.GetDatabase();
        _logger = logger;
        _configuration = configuration;

        _config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9093",
            GroupId = "email-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        _consumer.Subscribe("user_email");
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

            var subject = string.IsNullOrWhiteSpace(message.Subject) ? "SportHive" : message.Subject;
            var body = await BuildBodyAsync(message, subject);

            var mail = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var recipient in message.To.Split(',', StringSplitOptions.RemoveEmptyEntries))
                mail.To.Add(recipient.Trim());

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
                var cr = _consumer.Consume(stoppingToken);

                try
                {
                    var message = JsonSerializer.Deserialize<EnhancedEmailMessageDto>(
                        cr.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (message is null || string.IsNullOrWhiteSpace(message.To))
                    {
                        _logger.LogWarning("Invalid email message from Kafka: {Message}", cr.Message.Value);
                        _consumer.Commit(cr);
                        continue;
                    }

                    await SendEmail(message);
                    _consumer.Commit(cr);
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
            _consumer.Close();
        }
    }

    private async Task<string> BuildBodyAsync(EnhancedEmailMessageDto message, string subject)
    {
        if (NeedsGeneratedCode(message, subject))
        {
            var code = ResolveCode(message.Code);
            await _database.StringSetAsync(message.To, code, TimeSpan.FromMinutes(10));
            return HTMLTemplate.getHTMLPage(code, subject);
        }

        if (!string.IsNullOrWhiteSpace(message.Body))
            return message.IsBodyHtml ? message.Body : ToHtml(message.Body);

        return ToHtml("SportHive notification");
    }

    private static bool NeedsGeneratedCode(EnhancedEmailMessageDto message, string subject)
    {
        if (IsVerificationSubject(subject) && string.IsNullOrWhiteSpace(message.Body))
            return true;

        if (IsVerificationSubject(subject) && IsEmptyCodeBody(message.Body))
            return true;

        if (!string.IsNullOrWhiteSpace(message.Kind) &&
            (message.Kind.Contains("verification", StringComparison.OrdinalIgnoreCase) ||
             message.Kind.Contains("recovery", StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }

    private static bool IsVerificationSubject(string subject)
    {
        return subject.Contains("Підтвердження", StringComparison.OrdinalIgnoreCase) ||
               subject.Contains("Відновлення", StringComparison.OrdinalIgnoreCase) ||
               subject.Contains("verification", StringComparison.OrdinalIgnoreCase) ||
               subject.Contains("recovery", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsEmptyCodeBody(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return true;

        var text = WebUtility.HtmlDecode(Regex.Replace(body, "<.*?>", string.Empty)).Trim();

        return text.Equals("Ваш код:", StringComparison.OrdinalIgnoreCase) ||
               text.Equals("Ваш код", StringComparison.OrdinalIgnoreCase) ||
               text.EndsWith("Ваш код:", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveCode(string? code)
    {
        if (!string.IsNullOrWhiteSpace(code) && Regex.IsMatch(code, @"^\d{6}$"))
            return code;

        return Random.Shared.Next(100000, 1000000).ToString();
    }

    private static string ToHtml(string text)
    {
        return "<p>" + WebUtility.HtmlEncode(text).Replace("\n", "<br>") + "</p>";
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
    public string? Code { get; set; }
}
