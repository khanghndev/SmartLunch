using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;

namespace SmartLunch.Backend.Service.Application.Queries.Organizations.GetOrganizations;

/// <summary>
/// Query to get list of organizations with pagination
/// </summary>
public class GetOrganizationsQuery : IRequest<GetOrganizationsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetOrganizationsQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}
