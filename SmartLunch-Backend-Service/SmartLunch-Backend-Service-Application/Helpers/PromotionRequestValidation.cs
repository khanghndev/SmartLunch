using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Promotions;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class PromotionRequestValidation
{
    private static readonly HashSet<string> ScopeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        PromotionConstants.ScopeDish,
        PromotionConstants.ScopeOrderQuantity,
        PromotionConstants.ScopeCustomer,
    };

    private static readonly HashSet<string> DiscountTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        PromotionConstants.DiscountPercent,
        PromotionConstants.DiscountFixedAmount,
    };

    private static readonly HashSet<string> SelectionModes = new(StringComparer.OrdinalIgnoreCase)
    {
        PromotionConstants.SelectionFirstMatch,
        PromotionConstants.SelectionBestDiscount,
    };

    private static readonly HashSet<string> Channels = new(StringComparer.OrdinalIgnoreCase)
    {
        PromotionConstants.ChannelAll,
        PromotionConstants.ChannelB2C,
        PromotionConstants.ChannelB2BOrg,
    };

    public static void ValidateUpsert(UpsertPromotionRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Name is required.");

        if (!ScopeTypes.Contains(req.ScopeType))
            throw new ArgumentException("Invalid ScopeType.");

        if (!DiscountTypes.Contains(req.DiscountType))
            throw new ArgumentException("Invalid DiscountType.");

        if (!SelectionModes.Contains(req.SelectionMode))
            throw new ArgumentException("Invalid SelectionMode.");

        if (!Channels.Contains(req.Channel))
            throw new ArgumentException("Invalid Channel.");

        if (req.DiscountValue <= 0)
            throw new ArgumentException("DiscountValue must be greater than zero.");

        if (string.Equals(req.DiscountType, PromotionConstants.DiscountPercent, StringComparison.OrdinalIgnoreCase) &&
            req.DiscountValue > 100)
        {
            throw new ArgumentException("Percent discount cannot exceed 100.");
        }

        if (req.ValidTo < req.ValidFrom)
            throw new ArgumentException("ValidTo must be on or after ValidFrom.");

        if (string.Equals(req.ScopeType, PromotionConstants.ScopeOrderQuantity, StringComparison.OrdinalIgnoreCase) &&
            req.MinOrderQuantity is int q && q < 1)
        {
            throw new ArgumentException("MinOrderQuantity must be at least 1 for order_quantity scope.");
        }
    }
}
