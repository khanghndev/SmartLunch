using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.DeleteDishIngredient;

public class DeleteDishIngredientCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteDishIngredientCommand(int id)
    {
        Id = id;
    }
}
