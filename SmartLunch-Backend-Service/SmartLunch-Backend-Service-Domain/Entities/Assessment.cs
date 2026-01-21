namespace SmartLunch.Backend.Service.Domain.Entities;

public class Assessment
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string AssessmentType { get; set; } = "Quiz"; // Quiz, Assignment, Exam, Project
    public int TotalPoints { get; set; }
    public int TimeLimitMinutes { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? DueDate { get; set; }
    
    // Navigation properties
    public virtual Course Course { get; set; } = null!;
}
