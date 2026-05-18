using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Promotions.GetPromotion;

public class GetPromotionQueryHandler : IRequestHandler<GetPromotionQuery, GetPromotionResponse>
{
    private readonly IPromotionRepository _repository;

    public GetPromotionQueryHandler(IPromotionRepository repository) => _repository = repository;

    public async Task<GetPromotionResponse> Handle(GetPromotionQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("Invalid promotion id.");

        var entity = await _repository.GetByIdWithTargetsAsync(request.Id, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException($"Promotion with ID {request.Id} was not found.");

        return new GetPromotionResponse { Promotion = PromotionDtoMapping.ToDto(entity) };
    }
}
