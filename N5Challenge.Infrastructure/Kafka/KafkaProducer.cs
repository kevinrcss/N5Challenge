using Confluent.Kafka;
using Microsoft.Extensions.Options;
using N5Challenge.Infrastructure.Settings;

namespace N5Challenge.Infrastructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly ProducerConfig _config;

        public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
        {
            _config = new ProducerConfig { BootstrapServers = kafkaSettings.Value.BootstrapServers };
        }

        public async Task ProduceAsync(string topic, string key, string value)
        {
            using (var producer = new ProducerBuilder<string, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
            }
        }
    }
}
