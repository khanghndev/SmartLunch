using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Services;

public class RabbitMQEventConsumer : IEventConsumer
{
    public const string DefaultExchangeName = "smartlunch.events";
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQEventConsumer> _logger;
    private readonly string _exchangeName;
    private readonly string _queueName;

    public RabbitMQEventConsumer(
        IConnection connection,
        ILogger<RabbitMQEventConsumer> logger,
        string queueName,
        string exchangeName = DefaultExchangeName)
    {
        _connection = connection;
        _logger = logger;
        _queueName = queueName;
        _exchangeName = exchangeName;
    }

    public async Task ConsumeAsync<T>(string topic, Func<T, Task> handler, CancellationToken cancellationToken) where T : BaseEvent
    {
        using var channel = _connection.CreateModel();
        channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
        channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(_queueName, _exchangeName, topic);

        _logger.LogInformation("Subscribed to queue {QueueName}, routing key {Topic}", _queueName, topic);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) =>
        {
            try
            {
                var messageValue = Encoding.UTF8.GetString(ea.Body.Span);
                var eventMessage = JsonSerializer.Deserialize<T>(messageValue);
                if (eventMessage != null)
                {
                    handler(eventMessage).GetAwaiter().GetResult();
                    _logger.LogInformation("Processed message from routing key {Topic}", topic);
                }
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from {Topic}", topic);
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(_queueName, autoAck: false, consumer);

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer closed.");
        }
    }
}
