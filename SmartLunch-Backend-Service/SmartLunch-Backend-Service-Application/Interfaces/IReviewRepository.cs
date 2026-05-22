using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id);
    Task<Review?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Review> Reviews, int TotalCount)> GetReviewsAsync(int page, int pageSize, string? searchTerm = null);
    Task<(List<Review> Reviews, int TotalCount, double AverageRating)> GetManagerReviewsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? maxRating = null,
        CancellationToken cancellationToken = default);
    Task<(List<Review> Reviews, int TotalCount, double AverageRating)> GetPublicReviewsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<List<Order>> GetEnterpriseOrdersForUserAsync(int userId, IReadOnlyList<int> organizationIds, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(int userId, int? dishId, int? orderId, CancellationToken cancellationToken = default);
    Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}
