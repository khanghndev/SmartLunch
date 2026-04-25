namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class RevokePermissionFromUserRequest
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
}
