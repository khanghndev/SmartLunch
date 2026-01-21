using SmartLunch.Shared.MessageQueue.Dotnet.Events;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(string topic, T @event) where T : BaseEvent;
    }
}
