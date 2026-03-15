using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.DeleteDishIngredient;

public class DeleteDishIngredientCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteDishIngredientCommand(Guid id)
    {
        Id = id;
    }
}
