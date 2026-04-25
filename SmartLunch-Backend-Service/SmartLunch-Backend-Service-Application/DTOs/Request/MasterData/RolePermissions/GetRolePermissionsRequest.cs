namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;

public class GetRolePermissionsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? RoleId { get; set; }
    public int? PermissionId { get; set; }
    public bool? IsActive { get; set; }
}
