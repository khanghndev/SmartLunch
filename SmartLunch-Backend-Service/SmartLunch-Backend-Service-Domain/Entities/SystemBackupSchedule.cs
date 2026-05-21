namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Singleton cấu hình lịch sao lưu tự động (Id = 1).
/// </summary>
public class SystemBackupSchedule
{
    public int Id { get; set; } = 1;
    public bool IsEnabled { get; set; }
    /// <summary>Daily | Weekly | Once</summary>
    public string ScheduleMode { get; set; } = "Daily";
    /// <summary>Phút trong ngày theo giờ VN (0–1439), dùng cho Daily/Weekly.</summary>
    public int TimeOfDayMinutes { get; set; } = 180;
    /// <summary>0=CN … 6=T7, dùng khi ScheduleMode = Weekly.</summary>
    public int? DayOfWeek { get; set; }
    /// <summary>Thời điểm chạy một lần (giờ VN), dùng khi ScheduleMode = Once.</summary>
    public DateTime? OnceScheduledAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
}
