namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;

public class GetUserPermissionsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? PermissionId { get; set; }
    public bool? IsActive { get; set; }
}
