namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

public class CreateUserRoleResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Message { get; set; } = "User role created successfully";
}
