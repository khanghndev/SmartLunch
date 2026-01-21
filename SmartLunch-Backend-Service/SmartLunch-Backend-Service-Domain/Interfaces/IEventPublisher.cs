namespace SmartLunch.Backend.Service.Infrastructure.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topic, T message);
}