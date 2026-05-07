namespace SmartLunch.Backend.Service.Domain.Entities;

public class SystemBackup
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageBucket { get; set; } = string.Empty;
    public string StorageObjectName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RestoredAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

