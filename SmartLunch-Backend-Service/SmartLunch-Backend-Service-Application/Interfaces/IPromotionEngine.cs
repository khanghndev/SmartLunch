using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPromotionEngine
{
    Task<OrderPromotionEvaluateResult> EvaluateAsync(
        OrderPromotionEvaluateInput input,
        CancellationToken cancellationToken = default);

    OrderPromotionApplication BuildApplication(
        Order order,
        Promotion promotion,
        OrderPromotionEvaluateResult evaluation);
}
