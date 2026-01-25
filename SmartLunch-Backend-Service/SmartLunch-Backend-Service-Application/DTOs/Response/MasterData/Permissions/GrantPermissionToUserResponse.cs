namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

public class GrantPermissionToUserResponse
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string Message { get; set; } = "Permission granted successfully";
}
