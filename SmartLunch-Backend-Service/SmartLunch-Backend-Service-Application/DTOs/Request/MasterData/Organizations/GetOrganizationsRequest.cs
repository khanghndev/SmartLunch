namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Organizations;

public class GetOrganizationsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}
