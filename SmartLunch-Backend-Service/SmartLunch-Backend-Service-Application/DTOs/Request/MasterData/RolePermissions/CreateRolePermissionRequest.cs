namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;

public class CreateRolePermissionRequest
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public int? AssignedBy { get; set; }
}
