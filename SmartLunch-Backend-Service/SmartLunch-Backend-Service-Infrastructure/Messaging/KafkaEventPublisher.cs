using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using SmartLunch.Backend.Service.Infrastructure.Interfaces;

namespace SmartLunch.Backend.Service.Infrastructure.Messaging;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<Null, string> _producer;
    private readonly IConfiguration _configuration;

    public KafkaEventPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        var config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<Null, string>
            {
                Value = messageJson
            };

            await _producer.ProduceAsync(topic, kafkaMessage);
        }
        catch (ProduceException<Null, string> ex)
        {
            // Log error
            throw new Exception($"Failed to publish message to topic {topic}", ex);
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}

