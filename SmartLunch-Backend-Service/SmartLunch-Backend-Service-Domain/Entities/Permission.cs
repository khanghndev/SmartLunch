namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Permission entity for RBAC system
/// </summary>
public class Permission
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "users.create", "courses.read"
    public string? Description { get; set; }
    public string Resource { get; set; } = string.Empty; // e.g., "users", "courses"
    public string Action { get; set; } = string.Empty; // e.g., "create", "read", "update", "delete"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
