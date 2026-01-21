using Confluent.Kafka;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using SmartLunch.Shared.MessageQueue.Dotnet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, string bootstrapServers, string groupId)
        {
            // Cấu hình Producer
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            services.AddSingleton(producerConfig);
            services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

            // Cấu hình Consumer
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            services.AddSingleton(consumerConfig);
            services.AddSingleton<IEventConsumer, KafkaEventConsumer>();

            return services;
        }
    }
}