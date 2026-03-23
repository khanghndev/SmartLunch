using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.UpdateIngredientSource;

public class UpdateIngredientSourceCommandHandler : IRequestHandler<UpdateIngredientSourceCommand, GetIngredientSourceResponse>
{
    private readonly IIngredientSourceRepository _ingredientSourceRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IPartnerRepository _partnerRepository;

    public UpdateIngredientSourceCommandHandler(
        IIngredientSourceRepository ingredientSourceRepository,
        IIngredientRepository ingredientRepository,
        IPartnerRepository partnerRepository)
    {
        _ingredientSourceRepository = ingredientSourceRepository;
        _ingredientRepository = ingredientRepository;
        _partnerRepository = partnerRepository;
    }

    public async Task<GetIngredientSourceResponse> Handle(UpdateIngredientSourceCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _ingredientSourceRepository.GetByIdAsync(request.IngredientSourceId);
        if (entity == null)
            throw new KeyNotFoundException($"IngredientSource with ID {request.IngredientSourceId} was not found.");

        if (await _ingredientRepository.GetByIdAsync(req.IngredientId) == null)
            throw new KeyNotFoundException($"Ingredient with ID {req.IngredientId} was not found.");
        if (req.PartnerId.HasValue && await _partnerRepository.GetByIdAsync(req.PartnerId.Value) == null)
            throw new KeyNotFoundException($"Partner with ID {req.PartnerId} was not found.");

        entity.IngredientId = req.IngredientId;
        entity.PartnerId = req.PartnerId;
        entity.BatchNumber = string.IsNullOrWhiteSpace(req.BatchNumber) ? null : req.BatchNumber.Trim();
        entity.OriginDetails = string.IsNullOrWhiteSpace(req.OriginDetails) ? null : req.OriginDetails.Trim();
        entity.ProductionDate = req.ProductionDate;
        entity.ExpirationDate = req.ExpirationDate;
        entity.Certification = string.IsNullOrWhiteSpace(req.Certification) ? null : req.Certification.Trim();

        await _ingredientSourceRepository.UpdateAsync(entity);
        var reloaded = await _ingredientSourceRepository.GetByIdAsync(entity.Id);
        return new GetIngredientSourceResponse
        {
            IngredientSource = IngredientSourceDtoMapping.ToDto(reloaded ?? entity)
        };
    }
}
