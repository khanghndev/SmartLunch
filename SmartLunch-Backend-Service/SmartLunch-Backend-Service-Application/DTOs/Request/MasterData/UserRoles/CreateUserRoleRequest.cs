namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;

public class CreateUserRoleRequest
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public int? AssignedBy { get; set; }
}
