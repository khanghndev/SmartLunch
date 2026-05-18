namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Role entity for RBAC system
/// </summary>
public class Role
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystemRole { get; set; } = false; // System roles cannot be deleted
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
