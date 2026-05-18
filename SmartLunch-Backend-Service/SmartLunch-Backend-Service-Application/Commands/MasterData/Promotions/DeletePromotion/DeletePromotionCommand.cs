using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.DeletePromotion;

public record DeletePromotionCommand(int Id) : IRequest<Unit>;
