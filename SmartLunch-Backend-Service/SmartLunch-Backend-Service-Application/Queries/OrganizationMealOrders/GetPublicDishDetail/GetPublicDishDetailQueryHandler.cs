using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Queries.Dishes.GetDish;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetPublicDishDetail;

public sealed class GetPublicDishDetailQueryHandler : IRequestHandler<GetPublicDishDetailQuery, GetDishResponse>
{
    private readonly IMediator _mediator;

    public GetPublicDishDetailQueryHandler(IMediator mediator) => _mediator = mediator;

    public async Task<GetDishResponse> Handle(GetPublicDishDetailQuery request, CancellationToken cancellationToken)
    {
        if (request.DishId <= 0)
            throw new ArgumentException("Dish id is required.");

        var response = await _mediator.Send(
            new GetDishQuery(request.DishId, includeIngredientQuotas: true),
            cancellationToken);

        if (response.Dish.Id <= 0 || !response.Dish.IsActive)
            throw new KeyNotFoundException($"Dish {request.DishId} was not found.");

        return response;
    }
}
