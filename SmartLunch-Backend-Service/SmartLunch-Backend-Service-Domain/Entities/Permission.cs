namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Permission entity for RBAC system
/// </summary>
public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "users.create", "courses.read"
    public string? Description { get; set; }
    public string Resource { get; set; } = string.Empty; // e.g., "users", "courses"
    public string Action { get; set; } = string.Empty; // e.g., "create", "read", "update", "delete"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
