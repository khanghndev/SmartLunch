namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Roles;

public class GrantRoleToUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
