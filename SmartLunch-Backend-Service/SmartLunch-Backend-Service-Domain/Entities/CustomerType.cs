namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Customer profile type aligned with AI rules (org_*).
/// </summary>
public class CustomerType
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string ProfileKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual ICollection<WeeklyMenu> WeeklyMenus { get; set; } = new List<WeeklyMenu>();
}

