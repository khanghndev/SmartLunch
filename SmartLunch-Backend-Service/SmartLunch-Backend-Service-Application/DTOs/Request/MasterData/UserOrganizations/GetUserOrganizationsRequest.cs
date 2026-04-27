namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserOrganizations;

public class GetUserOrganizationsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? OrganizationId { get; set; }
    public bool? IsActive { get; set; }
}
