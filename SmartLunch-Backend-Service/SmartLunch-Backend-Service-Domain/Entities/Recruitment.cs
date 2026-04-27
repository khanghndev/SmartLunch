namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Recruitment entity
/// </summary>
public class Recruitment
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string JobType { get; set; } = "Full-time"; // Full-time | Part-time | Freelance
    public string? SalaryRange { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
}
