using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

namespace SmartLunch.Backend.Service.Application.Queries.Partners.GetPartners;

/// <summary>
/// Query to get list of partners with pagination
/// </summary>
public class GetPartnersQuery : IRequest<GetPartnersResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetPartnersQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}
