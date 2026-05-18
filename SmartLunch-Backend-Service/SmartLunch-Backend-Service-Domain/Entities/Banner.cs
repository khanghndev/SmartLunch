namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Banner entity - corresponds to banners table
/// </summary>
public class Banner
{
    public int Id { get; set; }

    /// <summary>
    /// Page name where banner is displayed (Home, Factory, etc.)
    /// </summary>
    public string PageName { get; set; } = string.Empty;

    /// <summary>
    /// Banner title (nullable in DB)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Banner subtitle (nullable in DB)
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Banner image URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Link when clicking the banner (nullable in DB)
    /// </summary>
    public string? CtaLink { get; set; }

    /// <summary>
    /// Display order (default: 0)
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Is the banner active (default: true)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time the banner was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    /// <summary>
    /// Date and time the banner was updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User Id that created this banner
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// User Id that last updated this banner
    /// </summary>
    public int? UpdatedBy { get; set; }
}