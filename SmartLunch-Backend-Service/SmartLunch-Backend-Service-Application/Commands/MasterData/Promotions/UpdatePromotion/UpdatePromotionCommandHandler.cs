using MediatR;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.CreatePromotion;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.UpdatePromotion;

public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, GetPromotionResponse>
{
    private readonly IPromotionRepository _repository;

    public UpdatePromotionCommandHandler(IPromotionRepository repository) => _repository = repository;

    public async Task<GetPromotionResponse> Handle(UpdatePromotionCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        PromotionRequestValidation.ValidateUpsert(req);

        var entity = await _repository.GetByIdWithTargetsAsync(command.Id, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException($"Promotion with ID {command.Id} was not found.");

        var code = string.IsNullOrWhiteSpace(req.Code) ? null : req.Code.Trim();
        if (code != null && await _repository.ExistsByCodeAsync(code, command.Id, cancellationToken))
            throw new ArgumentException($"Promotion code '{code}' already exists.");

        entity.Code = code;
        CreatePromotionCommandHandler.MapEntity(entity, req);
        await _repository.UpdateAsync(entity, cancellationToken);
        await _repository.ReplaceTargetsAsync(entity.Id, CreatePromotionCommandHandler.MapTargets(req), cancellationToken);

        var reloaded = await _repository.GetByIdWithTargetsAsync(entity.Id, cancellationToken);
        return new GetPromotionResponse { Promotion = PromotionDtoMapping.ToDto(reloaded ?? entity) };
    }
}
