// using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
// using SmartLunch.Shared.MessageQueue.Dotnet.Events;
// using SmartLunch.Shared.MessageQueue.Dotnet.Constants;
// using Microsoft.Extensions.Logging;

// namespace SmartLunch.Backend.Service.API.Services;

// /// <summary>
// /// Background service to consume Kafka messages
// /// </summary>
// public class KafkaConsumerBackgroundService : BackgroundService
// {
//     private readonly IEventConsumer _eventConsumer;
//     private readonly ILogger<KafkaConsumerBackgroundService> _logger;
//     private readonly IServiceProvider _serviceProvider;

//     public KafkaConsumerBackgroundService(
//         IEventConsumer eventConsumer,
//         ILogger<KafkaConsumerBackgroundService> logger,
//         IServiceProvider serviceProvider)
//     {
//         _eventConsumer = eventConsumer;
//         _logger = logger;
//         _serviceProvider = serviceProvider;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         _logger.LogInformation("Kafka Consumer Background Service is starting.");

//         // Start consuming from multiple topics in parallel
//         var tasks = new List<Task>
//         {
//             ConsumeUserEvents(stoppingToken),
//         };

//         await Task.WhenAny(tasks);
//     }

//     private async Task ConsumeUserEvents(CancellationToken cancellationToken)
//     {
//         try
//         {
//             await _eventConsumer.ConsumeAsync<UserCreatedEvent>(
//                 KafkaTopics.UserCreated,
//                 async (eventData) =>
//                 {
//                     _logger.LogInformation("Received UserCreatedEvent: UserId={UserId}, Username={Username}",
//                         eventData.UserId, eventData.Username);

//                     // Process the event (e.g., send welcome email, create audit log, etc.)
//                     using var scope = _serviceProvider.CreateScope();
//                     // Add your event handlers here
//                     await Task.CompletedTask;
//                 },
//                 cancellationToken);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error consuming user events");
//         }
//     }


//     public override async Task StopAsync(CancellationToken cancellationToken)
//     {
//         _logger.LogInformation("Kafka Consumer Background Service is stopping.");
//         await base.StopAsync(cancellationToken);
//     }
// }
