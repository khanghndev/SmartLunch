namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class RevokePermissionFromRoleRequest
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
