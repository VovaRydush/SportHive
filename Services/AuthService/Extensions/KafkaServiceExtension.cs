using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

public static class KafkaServiceExtensions
{
    public static void AddKafkaServices(this IServiceCollection services, string bootstrapServers)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };
        
        services.AddSingleton(producerConfig);

        services.AddSingleton<IProducer<Null, string>>(sp =>
        {
            var config = sp.GetRequiredService<ProducerConfig>();
            return new ProducerBuilder<Null, string>(config).Build();
        });
    }
}
