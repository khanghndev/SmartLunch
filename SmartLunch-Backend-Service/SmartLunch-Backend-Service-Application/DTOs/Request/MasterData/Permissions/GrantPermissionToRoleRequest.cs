namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class GrantPermissionToRoleRequest
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
