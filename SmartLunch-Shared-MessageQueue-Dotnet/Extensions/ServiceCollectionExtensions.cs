using RabbitMQ.Client;
using SmartLunch.Shared.MessageQueue.Dotnet.Interfaces;
using SmartLunch.Shared.MessageQueue.Dotnet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Shared.MessageQueue.Dotnet.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers RabbitMQ as the message broker: connection, publisher and consumer.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="hostName">RabbitMQ host (e.g. localhost or rabbitmq when in Docker)</param>
    /// <param name="queueName">Queue name for this consumer (e.g. smartlunch.backend)</param>
    /// <param name="port">Port (default 5672)</param>
    /// <param name="virtualHost">Virtual host (default /)</param>
    /// <param name="userName">Username</param>
    /// <param name="password">Password</param>
    /// <param name="exchangeName">Topic exchange name (default smartlunch.events)</param>
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        string hostName,
        string queueName,
        int port = 5672,
        string virtualHost = "/",
        string? userName = null,
        string? password = null,
        string exchangeName = RabbitMQEventPublisher.DefaultExchangeName)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            VirtualHost = virtualHost,
            DispatchConsumersAsync = false
        };
        if (!string.IsNullOrEmpty(userName))
            factory.UserName = userName;
        if (!string.IsNullOrEmpty(password))
            factory.Password = password;

        var connection = factory.CreateConnection();
        services.AddSingleton(connection);
        services.AddSingleton<IEventPublisher>(sp => new RabbitMQEventPublisher(
            sp.GetRequiredService<IConnection>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RabbitMQEventPublisher>>(),
            exchangeName));
        services.AddSingleton<IEventConsumer>(sp => new RabbitMQEventConsumer(
            sp.GetRequiredService<IConnection>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RabbitMQEventConsumer>>(),
            queueName,
            exchangeName));

        return services;
    }

    /// <summary>
    /// Registers RabbitMQ using an AMQP connection URI (e.g. amqp://user:pass@host:5672/vhost).
    /// </summary>
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        string connectionUri,
        string queueName,
        string exchangeName = RabbitMQEventPublisher.DefaultExchangeName)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionUri),
            DispatchConsumersAsync = false
        };
        var connection = factory.CreateConnection();
        services.AddSingleton(connection);
        services.AddSingleton<IEventPublisher>(sp => new RabbitMQEventPublisher(
            sp.GetRequiredService<IConnection>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RabbitMQEventPublisher>>(),
            exchangeName));
        services.AddSingleton<IEventConsumer>(sp => new RabbitMQEventConsumer(
            sp.GetRequiredService<IConnection>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RabbitMQEventConsumer>>(),
            queueName,
            exchangeName));

        return services;
    }
}
