using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class OrganizationMealContractOrderIndexVm
{
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? ApiError { get; set; }
    public DateOnly PeriodStartDefault { get; set; }
    public DateOnly PeriodEndDefault { get; set; }
    public DateOnly PeriodStartMin { get; set; }
    public DateOnly PeriodEndMax { get; set; }
    public string DateRuleHint { get; set; } = string.Empty;
    public OrganizationMealDeliveryDefaultsVm DeliveryDefaults { get; set; } = new();
}

public class OrganizationMealPeriodPreviewPromotionRequest
{
    public PrepareOrganizationMealPeriodContractClientRequest Order { get; set; } = new();
    public string? PromotionCode { get; set; }
    public int? PromotionId { get; set; }
}

public class OrganizationMealContractOrderReviewVm
{
    public PrepareOrganizationMealPeriodContractClientResponse Draft { get; set; } = new();
    public string OrganizationName { get; set; } = string.Empty;
    public int[] AllowedDepositPercents { get; set; } = { 20, 25, 30, 35, 50 };
}

public class OrganizationMealContractOrderPaymentResultVm
{
    public int OrderId { get; set; }
    public int ContractId { get; set; }
    public string? InvoiceCode { get; set; }
    public decimal TotalAmount { get; set; }
    public int DepositPercent { get; set; }
    public int DepositAmountVnd { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? PayOsMessage { get; set; }
    public bool PayOsReady { get; set; }
}

public class OrganizationMealContractListVm
{
    public string OrganizationName { get; set; } = string.Empty;
    public string? ApiError { get; set; }
    public List<PeriodContractListItemVm> Contracts { get; set; } = new();
}

public class PeriodContractListItemVm
{
    public int Id { get; set; }
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsDigitallySigned { get; set; }
    public int? SourceOrderId { get; set; }
    public int? MealsPerDay { get; set; }
    public string? ContractFileUrl { get; set; }
    public bool CanPayDeposit { get; set; }
    public bool CanSelectWeeklyMeals { get; set; }
}

public class OrganizationMealContractWeeklyVm
{
    public int ContractId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? ContractNumber { get; set; }
    public DateOnly ContractStart { get; set; }
    public DateOnly ContractEnd { get; set; }
    public int MealsPerDay { get; set; }
    public string? ApiError { get; set; }
    public List<ContractWeekVm> Weeks { get; set; } = new();
}

public class ContractWeekVm
{
    public DateOnly WeekMonday { get; set; }
    public DateOnly WeekEnd { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsPast { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsFuture { get; set; }
    public bool HasServiceDays { get; set; }
}
