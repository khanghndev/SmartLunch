using SmartLunch.Backend.Service.Application.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationMealOrderDraftCache
{
    Task<OrganizationMealOrderDraftPayload?> GetAsync(int userId, string draftId, CancellationToken cancellationToken = default);

    Task SaveAsync(int userId, string draftId, OrganizationMealOrderDraftPayload payload, CancellationToken cancellationToken = default);

    Task RemoveAsync(int userId, string draftId, CancellationToken cancellationToken = default);
}
