using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Promotions.GetPromotions;

public class GetPromotionsQueryHandler : IRequestHandler<GetPromotionsQuery, GetPromotionsResponse>
{
    private readonly IPromotionRepository _repository;

    public GetPromotionsQueryHandler(IPromotionRepository repository) => _repository = repository;

    public async Task<GetPromotionsResponse> Handle(GetPromotionsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page < 1) throw new ArgumentException("Page must be at least 1.");
        if (request.PageSize is < 1 or > 100) throw new ArgumentException("PageSize must be between 1 and 100.");

        var (items, total) = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            request.ScopeType,
            cancellationToken);

        return new GetPromotionsResponse
        {
            Data = items.Select(PromotionDtoMapping.ToDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total,
        };
    }
}
