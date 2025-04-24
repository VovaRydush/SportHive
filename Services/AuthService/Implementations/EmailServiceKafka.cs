using SportHive.Services.Interfaces;
using System.Net.Mail;
using Confluent.Kafka;
using System.Text.Json;
using DB.SportHive.Domain;


namespace SportHive.Implementations
{
    public class EmailServiceKafka : IEmailService
    {
        private readonly ProducerConfig _producerConfig;
        private IProducer<Null, string> _producer;

        public EmailServiceKafka(ProducerConfig config)
        {
            _producerConfig = config;
            _producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
        }

        public async Task SendEmail(EmailMessageDto message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync("user_email", new Message<Null, string>
            {
                Value = jsonMessage
            });
        }
    }
}