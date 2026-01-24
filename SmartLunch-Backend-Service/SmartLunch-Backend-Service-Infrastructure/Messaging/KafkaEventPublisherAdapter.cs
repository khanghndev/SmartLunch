using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;

namespace SmartLunch.Backend.Service.Infrastructure.Messaging;

/// <summary>
/// Adapter for Kafka event publisher
/// This adapter implements IEventPublisher from Shared project
/// </summary>
public class KafkaEventPublisherAdapter : IEventPublisher
{
    private readonly IEventPublisher _sharedPublisher;

    public KafkaEventPublisherAdapter(IEventPublisher sharedPublisher)
    {
        _sharedPublisher = sharedPublisher;
    }

    public Task PublishAsync<T>(string topic, T @event) where T : BaseEvent
    {
        return _sharedPublisher.PublishAsync(topic, @event);
    }
}
