using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly SmartLunchDBContext _context;

    public UserRoleRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<UserRole?> GetByIdAsync(int id)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.Id == id);
    }

    public async Task<UserRole?> GetByUserAndRoleAsync(int userId, int roleId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(int userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<UserRole> CreateAsync(UserRole userRole)
    {
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
        return userRole;
    }

    public async Task<UserRole> UpdateAsync(UserRole userRole)
    {
        _context.UserRoles.Update(userRole);
        await _context.SaveChangesAsync();
        return userRole;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);
        if (userRole == null) return false;

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByUserAndRoleAsync(int userId, int roleId)
    {
        var userRole = await GetByUserAndRoleAsync(userId, roleId);
        if (userRole == null) return false;

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByUserAndRoleAsync(int userId, int roleId)
    {
        return await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<(List<UserRole> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? roleId = null, bool? isActive = null)
    {
        var query = _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(ur => ur.UserId == userId.Value);
        if (roleId.HasValue)
            query = query.Where(ur => ur.RoleId == roleId.Value);
        if (isActive.HasValue)
            query = query.Where(ur => ur.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(ur => ur.AssignedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
