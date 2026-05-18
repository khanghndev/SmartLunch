namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// RolePermission junction entity for permission assignment to roles
/// </summary>
public class RolePermission
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = VietnamTime.Now;
    public int? AssignedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
