namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class RevokePermissionFromUserResponse
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string Message { get; set; } = "Permission revoked successfully";
}
