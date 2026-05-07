namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class GetSystemBackupFileResponse
{
    public int BackupId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageBucket { get; set; } = string.Empty;
    public string StorageObjectName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

