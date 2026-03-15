namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

public class UserPermissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string? UserName { get; set; }
    public string? PermissionName { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
    public bool IsActive { get; set; }
}
