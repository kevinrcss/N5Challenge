namespace N5Challenge.Infrastructure.Kafka
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string key, string value);
    }
}
