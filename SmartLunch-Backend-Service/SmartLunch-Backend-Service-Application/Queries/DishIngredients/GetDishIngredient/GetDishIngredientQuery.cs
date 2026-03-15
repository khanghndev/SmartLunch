using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

namespace SmartLunch.Backend.Service.Application.Queries.DishIngredients.GetDishIngredient;

public class GetDishIngredientQuery : IRequest<GetDishIngredientResponse>
{
    public Guid Id { get; set; }

    public GetDishIngredientQuery(Guid id)
    {
        Id = id;
    }
}
