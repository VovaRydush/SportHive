using System.Text;
using Confluent.Kafka;
using SportHive.Services.Interfaces;


namespace SportHive.Implementations
{
    public class SaveDataDb : ISaveDataDb
    {
        private readonly ProducerConfig _producerConfig;
        private IProducer<Null, string> _producer;

        public SaveDataDb(ProducerConfig config)
        {
            _producerConfig = config;
            _producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
        }
        public async Task SaveDataToDb(string jsonObj, string topic)
{
    List<string> topics = new()
    {
        "user_regist",
        "user-athlete",
        "user-trainer",
        "user-judge",
        "user-organization",
        "user-photo"
    };

    if (!topics.Contains(topic))
        throw new Exception("Topic not found");

    await _producer.ProduceAsync(topic, new Message<Null, string>
    {
        Value = jsonObj,
        Headers = new Headers
        {
            { "save-only-data", Encoding.UTF8.GetBytes("save-only-data") }
        }
    });
}
    }
}