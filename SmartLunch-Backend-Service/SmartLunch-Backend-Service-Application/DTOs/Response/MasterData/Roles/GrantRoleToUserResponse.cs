namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

public class GrantRoleToUserResponse
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string Message { get; set; } = "Role granted successfully";
}
