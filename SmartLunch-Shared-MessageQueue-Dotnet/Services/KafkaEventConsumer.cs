using Confluent.Kafka;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Services
{
    public class KafkaEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly ILogger<KafkaEventConsumer> _logger;

        public KafkaEventConsumer(ConsumerConfig config, ILogger<KafkaEventConsumer> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task ConsumeAsync<T>(string topic, Func<T, Task> handler, CancellationToken cancellationToken) where T : BaseEvent
        {
            using var consumer = new ConsumerBuilder<Null, string>(_config).Build();
            consumer.Subscribe(topic);

            _logger.LogInformation($"Subscribed to topic: {topic}");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 1. Nhận message (chờ tối đa 1 giây mỗi lần để không block thread)
                        var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

                        if (consumeResult != null)
                        {
                            // 2. Deserialize JSON thành Object
                            var eventMessage = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);

                            if (eventMessage != null)
                            {
                                // 3. Gọi hàm xử lý logic (do Service bên ngoài truyền vào)
                                await handler(eventMessage);
                                _logger.LogInformation($"Processed message from topic {topic}");
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
                _logger.LogInformation("Consumer closed.");
            }
        }
    }
}