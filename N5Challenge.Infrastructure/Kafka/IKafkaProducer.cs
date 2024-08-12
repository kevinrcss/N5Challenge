namespace N5Challenge.Infrastructure.Kafka
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, string key, T value);
    }
}
