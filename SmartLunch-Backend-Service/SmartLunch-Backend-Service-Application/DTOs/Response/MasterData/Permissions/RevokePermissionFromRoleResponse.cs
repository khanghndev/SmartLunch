namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class RevokePermissionFromRoleResponse
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public string Message { get; set; } = "Permission revoked from role successfully";
}
