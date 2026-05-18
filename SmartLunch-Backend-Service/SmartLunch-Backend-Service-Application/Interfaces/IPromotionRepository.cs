using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPromotionRepository
{
    Task<Promotion?> GetByIdWithTargetsAsync(int id, CancellationToken cancellationToken = default);
    Task<Promotion?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<(List<Promotion> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        bool? isActive,
        string? scopeType,
        CancellationToken cancellationToken = default);

    Task<List<Promotion>> GetActiveForEvaluationAsync(DateOnly today, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> CountTotalApplicationsAsync(int promotionId, CancellationToken cancellationToken = default);
    Task<int> CountUserApplicationsAsync(int promotionId, int userId, CancellationToken cancellationToken = default);
    Task<Promotion> CreateAsync(Promotion entity, CancellationToken cancellationToken = default);
    Task<Promotion> UpdateAsync(Promotion entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Promotion entity, CancellationToken cancellationToken = default);
    Task ReplaceTargetsAsync(int promotionId, List<PromotionTarget> targets, CancellationToken cancellationToken = default);
    Task AddApplicationAsync(OrderPromotionApplication application, CancellationToken cancellationToken = default);
}
