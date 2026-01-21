namespace SmartLunch.Backend.Service.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active"; // Active, Completed, Dropped
    public decimal? Progress { get; set; } = 0;
    
    // Navigation properties
    public virtual Course Course { get; set; } = null!;
}
