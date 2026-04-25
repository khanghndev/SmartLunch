namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

public class UserRoleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string? UserName { get; set; }
    public string? RoleName { get; set; }
    public DateTime AssignedAt { get; set; }
    public int? AssignedBy { get; set; }
    public bool IsActive { get; set; }
}
