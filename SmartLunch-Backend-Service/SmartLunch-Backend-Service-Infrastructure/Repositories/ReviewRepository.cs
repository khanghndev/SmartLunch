using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly SmartLunchDBContext _context;

    public ReviewRepository(SmartLunchDBContext context) => _context = context;

    private IQueryable<Review> WithDetails() =>
        _context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Order).ThenInclude(o => o!.Contract).ThenInclude(c => c!.Organization);

    public Task<Review?> GetByIdAsync(int id) =>
        _context.Reviews.FirstOrDefaultAsync(e => e.Id == id);

    public Task<Review?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Order).ThenInclude(o => o!.Contract).ThenInclude(c => c!.Organization)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<(List<Review> Reviews, int TotalCount)> GetReviewsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Reviews.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(e => e.Comment != null && e.Comment.Contains(searchTerm));

        var totalCount = await query.CountAsync();
        var reviews = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }

    public async Task<(List<Review> Reviews, int TotalCount, double AverageRating)> GetManagerReviewsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? maxRating = null,
        CancellationToken cancellationToken = default)
    {
        var query = WithDetails().Where(r => r.OrderId != null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(r =>
                (r.Comment != null && r.Comment.Contains(term))
                || (r.User.FirstName != null && r.User.FirstName.Contains(term))
                || (r.User.LastName != null && r.User.LastName.Contains(term))
                || r.User.Username.Contains(term)
                || (r.Order!.Code != null && r.Order.Code.Contains(term))
                || (r.Order.InvoiceCode != null && r.Order.InvoiceCode.Contains(term))
                || (r.Order.Contract != null && r.Order.Contract.Organization != null
                    && r.Order.Contract.Organization.Name.Contains(term)));
        }

        if (maxRating.HasValue)
            query = query.Where(r => r.Rating <= maxRating.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var avg = totalCount > 0
            ? await query.AverageAsync(r => (double)r.Rating, cancellationToken)
            : 0d;

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount, Math.Round(avg, 1));
    }

    public async Task<(List<Review> Reviews, int TotalCount, double AverageRating)> GetPublicReviewsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = WithDetails().Where(r => r.OrderId != null && !string.IsNullOrWhiteSpace(r.Comment));

        var totalCount = await query.CountAsync(cancellationToken);
        var avg = totalCount > 0
            ? await query.AverageAsync(r => (double)r.Rating, cancellationToken)
            : 0d;

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount, Math.Round(avg, 1));
    }

    public async Task<List<Order>> GetEnterpriseOrdersForUserAsync(
        int userId,
        IReadOnlyList<int> organizationIds,
        CancellationToken cancellationToken = default)
    {
        if (organizationIds.Count == 0)
            return new List<Order>();

        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Contract).ThenInclude(c => c!.Organization)
            .Where(o => o.UserId == userId
                        && o.ContractId != null
                        && o.Contract != null
                        && o.Contract.OrganizationId != null
                        && organizationIds.Contains(o.Contract.OrganizationId.Value))
            .OrderByDescending(o => o.ScheduledDate)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(
        int userId,
        int? dishId,
        int? orderId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Reviews.AsNoTracking().Where(r => r.UserId == userId);
        if (dishId != null)
            query = query.Where(r => r.DishId == dishId);
        if (orderId != null)
            query = query.Where(r => r.OrderId == orderId);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return review;
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
