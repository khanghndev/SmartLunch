using Confluent.Kafka;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Services
{
    public class KafkaEventPublisher : IEventPublisher
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaEventPublisher> _logger;

        public KafkaEventPublisher(ProducerConfig config, ILogger<KafkaEventPublisher> logger)
        {
            _logger = logger;
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T @event) where T : BaseEvent
        {
            var messageValue = JsonSerializer.Serialize(@event);
            var message = new Message<Null, string> { Value = messageValue };

            try
            {
                var result = await _producer.ProduceAsync(topic, message);
                _logger.LogInformation($"Message sent to {topic}: {result.Status}");
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Delivery failed: {e.Error.Reason}");
                throw;
            }
        }
    }
}