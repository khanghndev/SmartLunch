namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

public class CreateRolePermissionResponse
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public string Message { get; set; } = "Role permission created successfully";
}
