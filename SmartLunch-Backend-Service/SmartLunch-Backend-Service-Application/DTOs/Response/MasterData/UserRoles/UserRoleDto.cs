namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

public class UserRoleDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string? UserName { get; set; }
    public string? RoleName { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
    public bool IsActive { get; set; }
}
