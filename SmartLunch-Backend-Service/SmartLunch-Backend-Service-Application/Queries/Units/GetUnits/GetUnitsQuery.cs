using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Units;

namespace SmartLunch.Backend.Service.Application.Queries.Units.GetUnits;

/// <summary>
/// Query to get list of units with pagination
/// </summary>
public class GetUnitsQuery : IRequest<GetUnitsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetUnitsQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}
