namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;

public class UpdateBackupScheduleRequest
{
    public bool IsEnabled { get; set; }
    /// <summary>Daily | Weekly | Once</summary>
    public string ScheduleMode { get; set; } = "Daily";
    /// <summary>HH:mm theo giờ Việt Nam (Daily/Weekly).</summary>
    public string? TimeOfDay { get; set; }
    /// <summary>0=CN … 6=T7 (Weekly).</summary>
    public int? DayOfWeek { get; set; }
    /// <summary>datetime-local từ UI — giờ Việt Nam (Once).</summary>
    public DateTime? OnceScheduledAt { get; set; }
}
