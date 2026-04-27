namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class GetSystemBackupFileResponse
{
    public int BackupId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}

