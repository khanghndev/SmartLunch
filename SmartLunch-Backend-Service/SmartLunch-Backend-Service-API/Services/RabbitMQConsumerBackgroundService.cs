using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using SmartLunch.Shared.MessageQueue.Dotnet.Events;
using SmartLunch.Shared.MessageQueue.Dotnet.Constants;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.API.Services;

/// <summary>
/// Background service to consume RabbitMQ messages
/// </summary>
public class RabbitMQConsumerBackgroundService : BackgroundService
{
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<RabbitMQConsumerBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumerBackgroundService(
        IEventConsumer eventConsumer,
        ILogger<RabbitMQConsumerBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _eventConsumer = eventConsumer;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ Consumer Background Service is starting.");

        var tasks = new List<Task>
        {
            ConsumeUserEvents(stoppingToken),
        };

        await Task.WhenAll(tasks);
    }

    private async Task ConsumeUserEvents(CancellationToken cancellationToken)
    {
        try
        {
            await _eventConsumer.ConsumeAsync<UserCreatedEvent>(
                MessageQueueTopics.UserCreated,
                async (eventData) =>
                {
                    _logger.LogInformation("Received UserCreatedEvent: UserId={UserId}, Username={Username}",
                        eventData.UserId, eventData.Username);

                    using var scope = _serviceProvider.CreateScope();
                    // Add your event handlers here (e.g. send welcome email, create audit log)
                    await Task.CompletedTask;
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming user events");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Consumer Background Service is stopping.");
        await base.StopAsync(cancellationToken);
    }
}
