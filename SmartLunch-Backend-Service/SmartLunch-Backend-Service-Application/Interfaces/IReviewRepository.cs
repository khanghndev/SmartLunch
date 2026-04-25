using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id);
    Task<(List<Review> Reviews, int TotalCount)> GetReviewsAsync(int page, int pageSize, string? searchTerm = null);
}
