using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class SystemBackupRepository : ISystemBackupRepository
{
    private readonly SmartLunchDBContext _context;

    public SystemBackupRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<SystemBackup?> GetByIdAsync(int id)
    {
        return await _context.SystemBackups.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<SystemBackup?> GetLatestAsync()
    {
        return await _context.SystemBackups
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    public async Task<(List<SystemBackup> Backups, int TotalCount)> GetBackupsAsync(int page, int pageSize, bool includeDeleted = false)
    {
        var query = _context.SystemBackups.AsNoTracking().AsQueryable();
        if (!includeDeleted)
            query = query.Where(e => !e.IsDeleted);

        var totalCount = await query.CountAsync();

        var backups = await query
            .OrderByDescending(e => e.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (backups, totalCount);
    }

    public async Task<SystemBackup> CreateAsync(SystemBackup backup)
    {
        _context.SystemBackups.Add(backup);
        await _context.SaveChangesAsync();
        return backup;
    }

    public async Task<SystemBackup> UpdateAsync(SystemBackup backup)
    {
        _context.SystemBackups.Update(backup);
        await _context.SaveChangesAsync();
        return backup;
    }
}

