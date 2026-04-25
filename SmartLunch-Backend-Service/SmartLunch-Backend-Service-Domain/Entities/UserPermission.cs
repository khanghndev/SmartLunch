namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// UserPermission junction entity for direct permission assignment to users
/// </summary>
public class UserPermission
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public int? AssignedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
