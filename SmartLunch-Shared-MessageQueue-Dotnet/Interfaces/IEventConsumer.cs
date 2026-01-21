using SmartLunch.Shared.MessageQueue.Dotnet.Events;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Interfaces
{
    public interface IEventConsumer
    {
        Task ConsumeAsync<T>(string topic, Func<T, Task> handler, CancellationToken cancellationToken) where T : BaseEvent;
    }
}
