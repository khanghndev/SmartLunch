using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.DeleteIngredientSource;

public class DeleteIngredientSourceCommandHandler : IRequestHandler<DeleteIngredientSourceCommand, DeleteIngredientSourceResponse>
{
    private readonly IIngredientSourceRepository _ingredientSourceRepository;

    public DeleteIngredientSourceCommandHandler(IIngredientSourceRepository ingredientSourceRepository)
    {
        _ingredientSourceRepository = ingredientSourceRepository;
    }

    public async Task<DeleteIngredientSourceResponse> Handle(DeleteIngredientSourceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _ingredientSourceRepository.GetByIdAsync(request.IngredientSourceId);
        if (entity == null)
            throw new KeyNotFoundException($"IngredientSource with ID {request.IngredientSourceId} was not found.");

        await _ingredientSourceRepository.DeleteAsync(entity);
        return new DeleteIngredientSourceResponse
        {
            Id = request.IngredientSourceId,
            Message = "Ingredient source deleted successfully."
        };
    }
}
