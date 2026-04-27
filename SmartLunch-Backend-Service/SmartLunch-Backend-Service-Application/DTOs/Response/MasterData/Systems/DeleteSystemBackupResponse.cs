namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class DeleteSystemBackupResponse
{
    public int BackupId { get; set; }
    public bool IsDeleted { get; set; }
    public bool PhysicalFileDeleted { get; set; }
    public DateTime DeletedAtUtc { get; set; }
    public string? FilePath { get; set; }
}

