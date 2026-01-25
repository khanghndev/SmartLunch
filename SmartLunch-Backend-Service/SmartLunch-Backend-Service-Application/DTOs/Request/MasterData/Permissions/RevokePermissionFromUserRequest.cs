namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class RevokePermissionFromUserRequest
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
}
