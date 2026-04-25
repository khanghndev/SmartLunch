namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

public class CreateUserPermissionResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public string Message { get; set; } = "User permission created successfully";
}
