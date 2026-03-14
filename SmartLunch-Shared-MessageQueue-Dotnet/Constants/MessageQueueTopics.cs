namespace SmartLunch.Shared.MessageQueue.Dotnet.Constants;

/// <summary>
/// Topic / routing key names for the message broker (Kafka topic or RabbitMQ routing key).
/// </summary>
public static class MessageQueueTopics
{
    public const string UserCreated = "user.created";
    public const string CourseCreated = "course.created";
    public const string CoursePublished = "course.published";
}
