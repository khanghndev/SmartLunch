using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly SmartLunchDBContext _context;

    public ReviewRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Review> Reviews, int TotalCount)> GetReviewsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Reviews.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                (e.Comment != null && e.Comment.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var reviews = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }
}
