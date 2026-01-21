namespace SmartLunch.Backend.Service.Domain.Entities;

public class CourseContent
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty; // Video, Document, Quiz, Assignment
    public string FilePath { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Course Course { get; set; } = null!;
}
