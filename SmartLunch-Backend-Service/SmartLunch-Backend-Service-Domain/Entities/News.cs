namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// News entity
/// </summary>
public class News
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public int? AuthorId { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    public virtual User? Author { get; set; }
}