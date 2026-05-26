using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class OrganizationMealOrderIndexVm
{
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public GetOrganizationDishCategoriesClientResponse? Categories { get; set; }
    public string? ApiError { get; set; }
    public DateOnly DateMin { get; set; }
    public DateOnly DateMax { get; set; }
    public DateOnly DateDefault { get; set; }
    public string DateRuleHint { get; set; } = string.Empty;
    public bool FromCart { get; set; }
    public List<CartImportLineDto> CartImportLines { get; set; } = new();
    public OrganizationMealDeliveryDefaultsVm DeliveryDefaults { get; set; } = new();
}

/// <summary>Giá trị mặc định bước 4 — giao hàng (từ hồ sơ).</summary>
public class OrganizationMealDeliveryDefaultsVm
{
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientEmail { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? DeliveryWardDistrict { get; set; }
    public string? DeliveryNotes { get; set; }
    public string PreferredDeliveryTime { get; set; } = "11:30";
    public string? SourceHint { get; set; }
    public string? OrganizationName { get; set; }
}

public class OrganizationMealPreviewPromotionRequest
{
    public PrepareOrganizationMealContractClientRequest Order { get; set; } = new();
    public string? PromotionCode { get; set; }
    public int? PromotionId { get; set; }
}

public class OrganizationMealOrderReviewVm
{
    public PrepareOrganizationMealContractClientResponse Draft { get; set; } = new();
    public string OrganizationName { get; set; } = string.Empty;
    public int[] AllowedDepositPercents { get; set; } = { 20, 25, 30, 35, 50 };
}

public class OrganizationMealOrderPaymentResultVm
{
    public int OrderId { get; set; }
    public string? InvoiceCode { get; set; }
    public decimal TotalAmount { get; set; }
    public int DepositPercent { get; set; }
    public int DepositAmountVnd { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? PayOsMessage { get; set; }
    public bool PayOsReady { get; set; }
}
