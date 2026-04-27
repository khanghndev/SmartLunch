namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Represents a system log entry, matching the structure of the `system_logs` table.
/// </summary>
public class SystemLog
{
    public int Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? Level { get; set; }
    public string? Template { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}