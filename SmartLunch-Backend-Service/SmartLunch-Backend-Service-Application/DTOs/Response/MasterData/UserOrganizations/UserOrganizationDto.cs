namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;

public class UserOrganizationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public string? UserName { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}
