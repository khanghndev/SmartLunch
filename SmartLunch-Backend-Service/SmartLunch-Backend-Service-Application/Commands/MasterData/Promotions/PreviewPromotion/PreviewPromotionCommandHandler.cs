using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Promotions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.PreviewPromotion;

public class PreviewPromotionCommandHandler : IRequestHandler<PreviewPromotionCommand, PreviewPromotionResponse>
{
    private readonly IPromotionEngine _engine;

    public PreviewPromotionCommandHandler(IPromotionEngine engine) => _engine = engine;

    public async Task<PreviewPromotionResponse> Handle(PreviewPromotionCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (req.Subtotal <= 0)
            throw new ArgumentException("Subtotal must be greater than zero.");

        var channel = string.IsNullOrWhiteSpace(req.Channel)
            ? PromotionConstants.ChannelB2C
            : req.Channel.Trim().ToLowerInvariant();

        var input = new OrderPromotionEvaluateInput
        {
            Channel = channel,
            UserId = command.UserId,
            OrganizationId = req.OrganizationId,
            ContractId = req.ContractId,
            ContractType = req.ContractType,
            PromotionCode = req.PromotionCode,
            Subtotal = req.Subtotal,
            TotalQuantity = req.TotalQuantity,
            Lines = req.Lines.Select(l => new OrderPromotionLineInput
            {
                DishId = l.DishId,
                Quantity = l.Quantity,
                LineTotal = l.LineTotal,
            }).ToList(),
        };

        var result = await _engine.EvaluateAsync(input, cancellationToken);
        return PreviewPromotionResponse.From(result);
    }
}
