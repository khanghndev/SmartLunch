using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Services;

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    public const string DefaultExchangeName = "smartlunch.events";
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly string _exchangeName;

    public RabbitMQEventPublisher(
        IConnection connection,
        ILogger<RabbitMQEventPublisher> logger,
        string exchangeName = DefaultExchangeName)
    {
        _connection = connection;
        _logger = logger;
        _exchangeName = exchangeName;
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public Task PublishAsync<T>(string topic, T @event) where T : BaseEvent
    {
        var messageValue = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(messageValue);
        var routingKey = topic;

        _channel.BasicPublish(_exchangeName, routingKey, false, null, body.AsMemory());
        _logger.LogInformation("Message sent to exchange {Exchange}, routing key {RoutingKey}", _exchangeName, routingKey);
        return Task.CompletedTask;
    }

    public void Dispose() => _channel.Dispose();
}
