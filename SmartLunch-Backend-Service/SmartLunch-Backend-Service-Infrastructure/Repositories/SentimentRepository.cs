using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class SentimentRepository : ISentimentRepository
{
    private readonly SmartLunchDBContext _context;

    public SentimentRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Sentiment?> GetByIdAsync(int id)
    {
        return await _context.Sentiments
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Sentiment> Sentiments, int TotalCount)> GetSentimentsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Sentiments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.SentimentLabel.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var sentiments = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (sentiments, totalCount);
    }
}
