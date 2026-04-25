namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

public class UserPermissionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public string? UserName { get; set; }
    public string? PermissionName { get; set; }
    public DateTime AssignedAt { get; set; }
    public int? AssignedBy { get; set; }
    public bool IsActive { get; set; }
}
