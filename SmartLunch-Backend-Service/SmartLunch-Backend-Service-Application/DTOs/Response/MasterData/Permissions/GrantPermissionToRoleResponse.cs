namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class GrantPermissionToRoleResponse
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public string Message { get; set; } = "Permission granted to role successfully";
}
