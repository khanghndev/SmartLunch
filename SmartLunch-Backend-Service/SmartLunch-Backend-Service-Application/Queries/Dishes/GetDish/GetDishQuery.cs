using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDish;

/// <summary>
/// Query to get a dish by ID
/// </summary>
public class GetDishQuery : IRequest<GetDishResponse>
{
    public Guid DishId { get; set; }

    public GetDishQuery(Guid dishId)
    {
        DishId = dishId;
    }
}
