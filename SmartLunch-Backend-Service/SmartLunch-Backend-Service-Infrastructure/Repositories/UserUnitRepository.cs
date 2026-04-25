using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UserUnitRepository : IUserUnitRepository
{
    private readonly SmartLunchDBContext _context;

    public UserUnitRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<UserUnit?> GetByIdAsync(int id)
    {
        return await _context.UserUnits
            .Include(uu => uu.User)
            .Include(uu => uu.Unit)
            .FirstOrDefaultAsync(uu => uu.Id == id);
    }

    public async Task<UserUnit?> GetByUserAndUnitAsync(int userId, int unitId)
    {
        return await _context.UserUnits
            .Include(uu => uu.User)
            .Include(uu => uu.Unit)
            .FirstOrDefaultAsync(uu => uu.UserId == userId && uu.UnitId == unitId);
    }

    public async Task<IEnumerable<UserUnit>> GetByUserIdAsync(int userId)
    {
        return await _context.UserUnits
            .Include(uu => uu.User)
            .Include(uu => uu.Unit)
            .Where(uu => uu.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserUnit>> GetActiveByUserIdAsync(int userId)
    {
        return await _context.UserUnits
            .Include(uu => uu.User)
            .Include(uu => uu.Unit)
            .Where(uu => uu.UserId == userId && uu.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserUnit>> GetByUnitIdAsync(int unitId)
    {
        return await _context.UserUnits
            .Include(uu => uu.User)
            .Include(uu => uu.Unit)
            .Where(uu => uu.UnitId == unitId)
            .ToListAsync();
    }

    public async Task<(List<UserUnit> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? unitId = null, bool? isActive = null)
    {
        var query = _context.UserUnits
            .Include(uu => uu.User)
            .Include(uu => uu.Unit)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(uu => uu.UserId == userId.Value);
        if (unitId.HasValue)
            query = query.Where(uu => uu.UnitId == unitId.Value);
        if (isActive.HasValue)
            query = query.Where(uu => uu.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(uu => uu.JoinedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<UserUnit> CreateAsync(UserUnit userUnit)
    {
        _context.UserUnits.Add(userUnit);
        await _context.SaveChangesAsync();
        return userUnit;
    }

    public async Task<UserUnit> UpdateAsync(UserUnit userUnit)
    {
        _context.UserUnits.Update(userUnit);
        await _context.SaveChangesAsync();
        return userUnit;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userUnit = await _context.UserUnits.FindAsync(id);
        if (userUnit == null) return false;

        _context.UserUnits.Remove(userUnit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByUserAndUnitAsync(int userId, int unitId)
    {
        var userUnit = await GetByUserAndUnitAsync(userId, unitId);
        if (userUnit == null) return false;

        _context.UserUnits.Remove(userUnit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByUserAndUnitAsync(int userId, int unitId)
    {
        return await _context.UserUnits.AnyAsync(uu => uu.UserId == userId && uu.UnitId == unitId);
    }
}
