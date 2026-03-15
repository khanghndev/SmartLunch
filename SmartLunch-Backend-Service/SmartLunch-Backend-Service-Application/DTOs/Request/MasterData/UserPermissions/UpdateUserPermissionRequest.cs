namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;

public class UpdateUserPermissionRequest
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
