namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// WeeklyMenu image attachment (cover + gallery).
/// </summary>
public class WeeklyMenuImage
{
    public int Id { get; set; }
    public string? Code { get; set; }

    public int WeeklyMenuId { get; set; }
    public int MediaFileId { get; set; }

    /// <summary>
    /// cover | gallery
    /// </summary>
    public string Role { get; set; } = "gallery";
    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual WeeklyMenu WeeklyMenu { get; set; } = null!;
    public virtual MediaFile MediaFile { get; set; } = null!;
}

