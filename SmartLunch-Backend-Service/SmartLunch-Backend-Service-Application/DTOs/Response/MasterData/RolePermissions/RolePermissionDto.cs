namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

public class RolePermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public string? RoleName { get; set; }
    public string? PermissionName { get; set; }
    public DateTime AssignedAt { get; set; }
    public int? AssignedBy { get; set; }
    public bool IsActive { get; set; }
}
