namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Customer complaint
/// </summary>
public class Complaint
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "new"; // new|in_progress|resolved|rejected
    public int? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Order? Order { get; set; }
    public virtual User? AssignedToUser { get; set; }
}
