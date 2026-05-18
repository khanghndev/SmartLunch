using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.CreatePromotion;

public record CreatePromotionCommand(UpsertPromotionRequest Request) : IRequest<GetPromotionResponse>;
