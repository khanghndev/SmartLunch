namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Roles;

public class GrantRoleToUserRequest
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}
