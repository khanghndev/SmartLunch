using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;

namespace SmartLunch.Backend.Service.Application.Queries.Promotions.GetPromotion;

public record GetPromotionQuery(int Id) : IRequest<GetPromotionResponse>;
