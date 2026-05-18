using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.DeletePromotion;

public class DeletePromotionCommandHandler : IRequestHandler<DeletePromotionCommand, Unit>
{
    private readonly IPromotionRepository _repository;

    public DeletePromotionCommandHandler(IPromotionRepository repository) => _repository = repository;

    public async Task<Unit> Handle(DeletePromotionCommand command, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdWithTargetsAsync(command.Id, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException($"Promotion with ID {command.Id} was not found.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
