namespace SmartLunch.Backend.Service.Domain.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Published, Archived
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();
    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
