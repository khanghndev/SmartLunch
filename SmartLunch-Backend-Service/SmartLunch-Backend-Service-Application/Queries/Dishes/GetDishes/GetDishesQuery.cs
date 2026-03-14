using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDishes;

/// <summary>
/// Query to get list of dishes with pagination
/// </summary>
public class GetDishesQuery : IRequest<GetDishesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? Category { get; set; }

    public GetDishesQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, string? category = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
        Category = category;
    }
}
