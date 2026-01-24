namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// RolePermission junction entity for permission assignment to roles
/// </summary>
public class RolePermission
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
