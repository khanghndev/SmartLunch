using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Promotions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.ListEligiblePromotions;

public sealed class ListEligiblePromotionsCommandHandler
    : IRequestHandler<ListEligiblePromotionsCommand, ListEligiblePromotionsResponse>
{
    private readonly IPromotionEngine _engine;

    public ListEligiblePromotionsCommandHandler(IPromotionEngine engine) => _engine = engine;

    public async Task<ListEligiblePromotionsResponse> Handle(
        ListEligiblePromotionsCommand command,
        CancellationToken cancellationToken)
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
            Subtotal = req.Subtotal,
            TotalQuantity = req.TotalQuantity,
            Lines = req.Lines.Select(l => new OrderPromotionLineInput
            {
                DishId = l.DishId,
                Quantity = l.Quantity,
                LineTotal = l.LineTotal,
            }).ToList(),
        };

        var result = await _engine.ListEligibleAsync(input, cancellationToken);
        return ListEligiblePromotionsResponse.From(result);
    }
}
