using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetDishSuggestions;

public sealed class GetDishSuggestionsQuery : IRequest<GetDishSuggestionsResponse>
{
    public GetDishSuggestionsQuery(int dishId, int maxItems = 4)
    {
        DishId = dishId;
        MaxItems = Math.Clamp(maxItems, 1, 8);
    }

    public int DishId { get; }
    public int MaxItems { get; }
}
