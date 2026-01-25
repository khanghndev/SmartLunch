namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class RevokePermissionFromRoleRequest
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
