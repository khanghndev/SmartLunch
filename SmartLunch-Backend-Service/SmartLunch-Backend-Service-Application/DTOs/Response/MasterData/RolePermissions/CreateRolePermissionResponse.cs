namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

public class CreateRolePermissionResponse
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public string Message { get; set; } = "Role permission created successfully";
}
