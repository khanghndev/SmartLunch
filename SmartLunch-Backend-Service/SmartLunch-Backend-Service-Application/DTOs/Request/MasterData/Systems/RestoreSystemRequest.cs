namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;

public class RestoreSystemRequest
{
    /// <summary>
    /// Preferred way for Admin UI: select backup record from DB.
    /// </summary>
    public int? BackupId { get; set; }

    /// <summary>
    /// Optional. If omitted, the API will restore from the latest backup file in configured backup directory.
    /// </summary>
    public string? FilePath { get; set; }
}

