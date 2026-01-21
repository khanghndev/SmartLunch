namespace SmartLunch.Shared.MessageQueue.Dotnet.Events
{
    public abstract class BaseEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}