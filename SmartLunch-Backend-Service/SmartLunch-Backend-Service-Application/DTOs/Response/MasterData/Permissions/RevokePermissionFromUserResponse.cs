namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class RevokePermissionFromUserResponse
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public string Message { get; set; } = "Permission revoked successfully";
}
