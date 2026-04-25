namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class GrantPermissionToRoleResponse
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public string Message { get; set; } = "Permission granted to role successfully";
}
