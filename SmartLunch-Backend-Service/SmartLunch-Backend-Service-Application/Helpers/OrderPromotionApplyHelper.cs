using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class OrderPromotionApplyHelper
{
    public static async Task<OrderPromotionEvaluateResult> EvaluateAndApplyToOrderAsync(
        Order order,
        OrderPromotionEvaluateInput input,
        IPromotionEngine engine,
        IPromotionRepository promotionRepository,
        CancellationToken cancellationToken = default)
    {
        var evaluation = await engine.EvaluateAsync(input, cancellationToken);

        order.SubtotalAmount = evaluation.Subtotal;
        order.DiscountAmount = evaluation.DiscountAmount;
        order.TotalAmount = evaluation.TotalAfter;

        if (!evaluation.Applied || evaluation.PromotionId is not int promoId)
            return evaluation;

        var promo = await promotionRepository.GetByIdWithTargetsAsync(promoId, cancellationToken);
        if (promo == null)
            return evaluation;

        var application = engine.BuildApplication(order, promo, evaluation);
        order.PromotionApplications.Add(application);
        return evaluation;
    }
}
