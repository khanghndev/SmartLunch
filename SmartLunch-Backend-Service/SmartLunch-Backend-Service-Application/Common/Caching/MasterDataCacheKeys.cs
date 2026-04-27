using System;

namespace SmartLunch.Backend.Service.Application.Common.Caching;

public static class MasterDataCacheKeys
{
    public static string Dish(int dishId) => $"master-data:dishes:detail:{dishId}";

    public static string Dishes(int page, int pageSize, string? searchTerm, bool? isActive, string? category) =>
        $"master-data:dishes:list:page={page}:pageSize={pageSize}:search={Normalize(searchTerm)}:active={Normalize(isActive)}:category={Normalize(category)}";

    public static string Partner(int partnerId) => $"master-data:partners:detail:{partnerId}";

    public static string Partners(int page, int pageSize, string? searchTerm, bool? isActive) =>
        $"master-data:partners:list:page={page}:pageSize={pageSize}:search={Normalize(searchTerm)}:active={Normalize(isActive)}";

    public static string Organization(int organizationId) => $"master-data:organizations:detail:{organizationId}";

    public static string Organizations(int page, int pageSize, string? searchTerm, bool? isActive) =>
        $"master-data:organizations:list:page={page}:pageSize={pageSize}:search={Normalize(searchTerm)}:active={Normalize(isActive)}";

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "all";
        }

        return Uri.EscapeDataString(value.Trim().ToLowerInvariant());
    }

    private static string Normalize(bool? value) =>
        value.HasValue ? value.Value.ToString().ToLowerInvariant() : "all";
}
