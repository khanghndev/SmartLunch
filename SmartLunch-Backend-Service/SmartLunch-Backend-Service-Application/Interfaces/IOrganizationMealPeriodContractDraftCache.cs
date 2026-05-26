using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationMealPeriodContractDraftCache
{
    Task<OrganizationMealPeriodContractDraftPayload?> GetAsync(int userId, string draftId, CancellationToken cancellationToken = default);
    Task SaveAsync(int userId, string draftId, OrganizationMealPeriodContractDraftPayload payload, CancellationToken cancellationToken = default);
    Task RemoveAsync(int userId, string draftId, CancellationToken cancellationToken = default);
}
