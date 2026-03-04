namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Weekly menu
/// </summary>
public class WeeklyMenu
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User CreatedByUser { get; set; } = null!;
    public virtual ICollection<MenuSchedule> MenuSchedules { get; set; } = new List<MenuSchedule>();
}
