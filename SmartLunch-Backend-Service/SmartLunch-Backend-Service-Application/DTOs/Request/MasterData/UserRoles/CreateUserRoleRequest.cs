namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;

public class CreateUserRoleRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? AssignedBy { get; set; }
}
