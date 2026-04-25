namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class GrantPermissionToUserRequest
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
}
