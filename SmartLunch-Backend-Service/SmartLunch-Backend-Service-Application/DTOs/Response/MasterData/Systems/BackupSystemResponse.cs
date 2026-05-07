namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class BackupSystemResponse
{
    public string FileName { get; set; } = string.Empty;
    public string StorageBucket { get; set; } = string.Empty;
    public string StorageObjectName { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
    public DateTime? DownloadUrlExpiresAtUtc { get; set; }
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

