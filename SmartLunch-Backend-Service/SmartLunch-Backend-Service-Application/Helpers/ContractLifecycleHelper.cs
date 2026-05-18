using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class ContractLifecycleHelper
{
    public static void ApplyExpiryByEndDate(Contract contract)
    {
        if (string.Equals(contract.Status, ContractStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            return;
        if (contract.EndDate.HasValue && contract.EndDate.Value.Date < VietnamTime.Today.ToDateTime(TimeOnly.MinValue))
            contract.Status = ContractStatus.Expired;
    }
}
