namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

public class RevokeRoleFromUserResponse
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string Message { get; set; } = "Role revoked successfully";
}
