namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;

public class UpdateUserRoleRequest
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
