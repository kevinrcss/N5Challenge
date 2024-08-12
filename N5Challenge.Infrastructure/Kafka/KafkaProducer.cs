using Confluent.Kafka;
using Microsoft.Extensions.Options;
using N5Challenge.Infrastructure.Settings;
using System.Text.Json;

namespace N5Challenge.Infrastructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly ProducerConfig _config;

        public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
        {
            _config = new ProducerConfig { BootstrapServers = kafkaSettings.Value.BootstrapServers };
        }

        public async Task ProduceAsync<T>(string topic, string key, T value)
        {
            using var producer = new ProducerBuilder<string, string>(_config).Build();
            var serializedValue = JsonSerializer.Serialize(value);
            await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = serializedValue });
        }
    }
}
