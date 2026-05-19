using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.ListEligiblePromotions;

public record ListEligiblePromotionsCommand(PreviewPromotionRequest Request, int? UserId)
    : IRequest<ListEligiblePromotionsResponse>;
