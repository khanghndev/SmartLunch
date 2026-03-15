using SmartLunch.Shared.MessageQueue.Dotnet.Events;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;

namespace SmartLunch.Backend.Service.API.Services;

/// <summary>
/// No-op event publisher used when RabbitMQ is unavailable. Events are not sent anywhere.
/// </summary>
public sealed class NoOpEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(string topic, T @event) where T : BaseEvent => Task.CompletedTask;
}

/// <summary>
/// No-op event consumer used when RabbitMQ is unavailable. No messages are consumed.
/// </summary>
public sealed class NoOpEventConsumer : IEventConsumer
{
    public Task ConsumeAsync<T>(string topic, Func<T, Task> handler, CancellationToken cancellationToken) where T : BaseEvent => Task.CompletedTask;
}
