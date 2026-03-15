namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

public class RolePermissionDto
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public string? RoleName { get; set; }
    public string? PermissionName { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
    public bool IsActive { get; set; }
}
