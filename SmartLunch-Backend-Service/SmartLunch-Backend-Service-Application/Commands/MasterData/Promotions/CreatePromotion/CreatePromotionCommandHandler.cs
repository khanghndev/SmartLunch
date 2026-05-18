using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.CreatePromotion;

public class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, GetPromotionResponse>
{
    private readonly IPromotionRepository _repository;

    public CreatePromotionCommandHandler(IPromotionRepository repository) => _repository = repository;

    public async Task<GetPromotionResponse> Handle(CreatePromotionCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        PromotionRequestValidation.ValidateUpsert(req);

        var code = string.IsNullOrWhiteSpace(req.Code) ? null : req.Code.Trim();
        if (code != null && await _repository.ExistsByCodeAsync(code, null, cancellationToken))
            throw new ArgumentException($"Promotion code '{code}' already exists.");

        var entity = MapEntity(new Promotion(), req);
        entity.Code = code;
        entity.CreatedAt = VietnamTime.Now;

        var created = await _repository.CreateAsync(entity, cancellationToken);
        await _repository.ReplaceTargetsAsync(created.Id, MapTargets(req), cancellationToken);

        var reloaded = await _repository.GetByIdWithTargetsAsync(created.Id, cancellationToken);
        return new GetPromotionResponse { Promotion = PromotionDtoMapping.ToDto(reloaded ?? created) };
    }

    internal static Promotion MapEntity(Promotion entity, DTOs.Request.MasterData.Promotions.UpsertPromotionRequest req)
    {
        entity.Name = req.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        entity.ScopeType = req.ScopeType.Trim().ToLowerInvariant();
        entity.DiscountType = req.DiscountType.Trim().ToLowerInvariant();
        entity.DiscountValue = req.DiscountValue;
        entity.Priority = req.Priority;
        entity.SelectionMode = req.SelectionMode.Trim().ToLowerInvariant();
        entity.Channel = req.Channel.Trim().ToLowerInvariant();
        entity.MinOrderQuantity = req.MinOrderQuantity;
        entity.MinOrderAmount = req.MinOrderAmount;
        entity.ValidFrom = req.ValidFrom;
        entity.ValidTo = req.ValidTo;
        entity.BookingTimeStart = req.BookingTimeStart;
        entity.BookingTimeEnd = req.BookingTimeEnd;
        entity.MaxTotalUses = req.MaxTotalUses;
        entity.MaxUsesPerUser = req.MaxUsesPerUser;
        entity.IsActive = req.IsActive;
        entity.UpdatedAt = VietnamTime.Now;
        return entity;
    }

    internal static List<PromotionTarget> MapTargets(DTOs.Request.MasterData.Promotions.UpsertPromotionRequest req) =>
        req.Targets.Select(t => new PromotionTarget
        {
            TargetType = t.TargetType.Trim().ToLowerInvariant(),
            TargetId = t.TargetId,
            TargetKey = string.IsNullOrWhiteSpace(t.TargetKey) ? null : t.TargetKey.Trim(),
        }).ToList();
}
