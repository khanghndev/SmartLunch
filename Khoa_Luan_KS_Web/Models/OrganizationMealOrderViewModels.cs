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
