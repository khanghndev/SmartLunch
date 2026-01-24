namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// UserRole junction entity for many-to-many relationship between User and Role
/// </summary>
public class UserRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
