namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Dish image attachment (cover + gallery).
/// Backed by media_files row for storage metadata.
/// </summary>
public class DishImage
{
    public int Id { get; set; }
    public string? Code { get; set; }

    public int DishId { get; set; }
    public int MediaFileId { get; set; }

    /// <summary>
    /// cover | gallery
    /// </summary>
    public string Role { get; set; } = "gallery";
    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Dish Dish { get; set; } = null!;
    public virtual MediaFile MediaFile { get; set; } = null!;
}

