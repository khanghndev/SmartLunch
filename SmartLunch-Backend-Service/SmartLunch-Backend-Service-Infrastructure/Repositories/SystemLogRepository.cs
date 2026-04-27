using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class SystemLogRepository : ISystemLogRepository
{
    private readonly SmartLunchDBContext _context;

    public SystemLogRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<(List<SystemLog> Logs, int TotalCount)> GetSystemLogsAsync(int page, int pageSize)
    {
        var query = _context.SystemLogs.AsNoTracking();

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (logs, totalCount);
    }

    public async Task<List<SystemLog>> GetAllAsync(int maxRows = 10000)
    {
        maxRows = Math.Clamp(maxRows, 1, 100000);
        return await _context.SystemLogs
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Take(maxRows)
            .ToListAsync();
    }

    public async Task<int> BulkInsertAsync(IEnumerable<SystemLog> logs)
    {
        var list = logs.ToList();
        if (list.Count == 0) return 0;

        await _context.SystemLogs.AddRangeAsync(list);
        return await _context.SaveChangesAsync();
    }
}

