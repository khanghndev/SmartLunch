namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;

public class CreateUserPermissionRequest
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public int? AssignedBy { get; set; }
}
