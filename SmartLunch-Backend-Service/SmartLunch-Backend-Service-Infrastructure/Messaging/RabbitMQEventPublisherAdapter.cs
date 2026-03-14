using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;

namespace SmartLunch.Backend.Service.Infrastructure.Messaging;

/// <summary>
/// Adapter for RabbitMQ event publisher (forwards to Shared IEventPublisher implementation).
/// </summary>
public class RabbitMQEventPublisherAdapter : IEventPublisher
{
    private readonly IEventPublisher _sharedPublisher;

    public RabbitMQEventPublisherAdapter(IEventPublisher sharedPublisher)
    {
        _sharedPublisher = sharedPublisher;
    }

    public Task PublishAsync<T>(string topic, T @event) where T : BaseEvent
    {
        return _sharedPublisher.PublishAsync(topic, @event);
    }
}
