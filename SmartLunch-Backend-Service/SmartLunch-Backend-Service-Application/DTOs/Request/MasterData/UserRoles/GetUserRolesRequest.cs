namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;

public class GetUserRolesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? RoleId { get; set; }
    public bool? IsActive { get; set; }
}
