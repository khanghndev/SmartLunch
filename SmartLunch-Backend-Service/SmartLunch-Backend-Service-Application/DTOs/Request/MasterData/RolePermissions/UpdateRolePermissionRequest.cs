namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;

public class UpdateRolePermissionRequest
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
