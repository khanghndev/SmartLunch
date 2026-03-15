using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.DeleteDishIngredient;

public class DeleteDishIngredientCommandHandler : IRequestHandler<DeleteDishIngredientCommand, bool>
{
    private readonly IDishIngredientRepository _dishIngredientRepository;

    public DeleteDishIngredientCommandHandler(IDishIngredientRepository dishIngredientRepository)
    {
        _dishIngredientRepository = dishIngredientRepository;
    }

    public async Task<bool> Handle(DeleteDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _dishIngredientRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"DishIngredient with ID {request.Id} not found");
        return true;
    }
}
