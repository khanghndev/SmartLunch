namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

public class CreateUserPermissionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string Message { get; set; } = "User permission created successfully";
}
