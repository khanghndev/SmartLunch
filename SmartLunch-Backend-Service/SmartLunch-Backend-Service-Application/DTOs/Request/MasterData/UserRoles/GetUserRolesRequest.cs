namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;

public class GetUserRolesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public bool? IsActive { get; set; }
}
