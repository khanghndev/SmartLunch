namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class GrantPermissionToUserResponse
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public string Message { get; set; } = "Permission granted successfully";
}
