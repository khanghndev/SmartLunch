namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Weekly menu
/// </summary>
public class WeeklyMenu
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string MenuType { get; set; } = "General";
    public int? CustomerTypeId { get; set; }
    public string? Description { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User CreatedByUser { get; set; } = null!;
    public virtual CustomerType? CustomerType { get; set; }
    public virtual ICollection<MenuSchedule> MenuSchedules { get; set; } = new List<MenuSchedule>();
    public virtual ICollection<WeeklyMenuImage> WeeklyMenuImages { get; set; } = new List<WeeklyMenuImage>();
}
