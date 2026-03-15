namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

public class CreateUserRoleResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string Message { get; set; } = "User role created successfully";
}
