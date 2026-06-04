namespace SmartLunch.Backend.Service.Application.Constants;

public static class OrganizationMealContractTypes
{
    public const string OrderBased = "Order-Based";
    public const string PeriodBased = "Period-Based";

    public static bool IsPeriodBased(string? contractType) =>
        string.Equals((contractType ?? "").Trim(), PeriodBased, StringComparison.OrdinalIgnoreCase);

    public static bool IsOrderBased(string? contractType) =>
        string.Equals((contractType ?? "").Trim(), OrderBased, StringComparison.OrdinalIgnoreCase);
}
