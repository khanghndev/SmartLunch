using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetPublicDishDetail;

public sealed class GetPublicDishDetailQuery : IRequest<GetDishResponse>
{
    public GetPublicDishDetailQuery(int dishId) => DishId = dishId;
    public int DishId { get; }
}
