namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class BackupSystemResponse
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

