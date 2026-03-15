namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;

public class CreateUserPermissionRequest
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public Guid? AssignedBy { get; set; }
}
