using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;

namespace SmartLunch.Backend.Service.Application.Queries.Promotions.GetPromotions;

public record GetPromotionsQuery(int Page, int PageSize, string? SearchTerm, bool? IsActive, string? ScopeType)
    : IRequest<GetPromotionsResponse>;
