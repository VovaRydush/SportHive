using System.Text;
using Confluent.Kafka;
using SportHive.Services.Interfaces;


namespace SportHive.Implementations
{
    class SaveDataDb : ISaveDataDb
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
            switch (topic)
            {
                case "user_regist":
                    await _producer.ProduceAsync("user_regist", new Message<Null, string>
                    {
                        Value = jsonObj,
                        Headers = new Headers{
                            {"save-only-data", Encoding.UTF8.GetBytes("save-only-data")}
                        }
                    });
                    break;

                case "user-athlete":
                    await _producer.ProduceAsync("user-athlete", new Message<Null, string>
                    {
                        Value = jsonObj,
                        Headers = new Headers{
                            {"save-only-data", Encoding.UTF8.GetBytes("save-only-data")}
                        }
                    });
                    break;

                case "user-trainer":
                    await _producer.ProduceAsync("user-trainer", new Message<Null, string>
                    {
                        Value = jsonObj,
                         Headers = new Headers{
                            {"save-only-data", Encoding.UTF8.GetBytes("save-only-data")}
                        }
                    });
                    break;

                case "user-judge":
                    await _producer.ProduceAsync("user-judge", new Message<Null, string>
                    {
                        Value = jsonObj,
                         Headers = new Headers{
                            {"save-only-data", Encoding.UTF8.GetBytes("save-only-data")}
                        }
                    });
                    break;

                case "user-photo":
                    await _producer.ProduceAsync("user-photo", new Message<Null, string>
                    {
                        Value = jsonObj,
                         Headers = new Headers{
                            {"save-only-data", Encoding.UTF8.GetBytes("save-only-data")}
                        }
                    });
                    break;

                case "user-organization":
                    await _producer.ProduceAsync("user-organization", new Message<Null, string>
                    {
                        Value = jsonObj,
                         Headers = new Headers{
                            {"save-only-data", Encoding.UTF8.GetBytes("save-only-data")}
                        }
                    });
                    break;
            }
        }
    }
}