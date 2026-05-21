namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class BackupScheduleDto
{
    public bool IsEnabled { get; set; }
    public string ScheduleMode { get; set; } = "Daily";
    public string TimeOfDay { get; set; } = "03:00";
    public int? DayOfWeek { get; set; }
    public DateTime? OnceScheduledAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public DateTime? LatestBackupAt { get; set; }
    public string? LatestBackupFileName { get; set; }
    public long TotalBackupBytes { get; set; }
    public int TotalBackupCount { get; set; }
    public string StorageProvider { get; set; } = "Appwrite";
}
