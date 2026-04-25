namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

public class GrantRoleToUserResponse
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Message { get; set; } = "Role granted successfully";
}
