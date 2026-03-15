namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;

public class CreateRolePermissionRequest
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Guid? AssignedBy { get; set; }
}
