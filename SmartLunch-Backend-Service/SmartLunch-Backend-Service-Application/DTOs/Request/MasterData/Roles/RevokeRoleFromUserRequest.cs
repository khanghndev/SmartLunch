namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Roles;

public class RevokeRoleFromUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
