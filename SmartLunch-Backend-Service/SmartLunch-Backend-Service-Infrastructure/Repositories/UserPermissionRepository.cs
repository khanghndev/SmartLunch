using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly SmartLunchDBContext _context;

    public UserPermissionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<UserPermission?> GetByIdAsync(Guid id)
    {
        return await _context.UserPermissions
            .Include(up => up.User)
            .Include(up => up.Permission)
            .FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<UserPermission?> GetByUserAndPermissionAsync(Guid userId, Guid permissionId)
    {
        return await _context.UserPermissions
            .Include(up => up.User)
            .Include(up => up.Permission)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
    }

    public async Task<IEnumerable<UserPermission>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserPermissions
            .Include(up => up.User)
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPermission>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.UserPermissions
            .Include(up => up.User)
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId && up.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPermission>> GetByPermissionIdAsync(Guid permissionId)
    {
        return await _context.UserPermissions
            .Include(up => up.User)
            .Include(up => up.Permission)
            .Where(up => up.PermissionId == permissionId)
            .ToListAsync();
    }

    public async Task<UserPermission> CreateAsync(UserPermission userPermission)
    {
        _context.UserPermissions.Add(userPermission);
        await _context.SaveChangesAsync();
        return userPermission;
    }

    public async Task<UserPermission> UpdateAsync(UserPermission userPermission)
    {
        _context.UserPermissions.Update(userPermission);
        await _context.SaveChangesAsync();
        return userPermission;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userPermission = await _context.UserPermissions.FindAsync(id);
        if (userPermission == null) return false;

        _context.UserPermissions.Remove(userPermission);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByUserAndPermissionAsync(Guid userId, Guid permissionId)
    {
        var userPermission = await GetByUserAndPermissionAsync(userId, permissionId);
        if (userPermission == null) return false;

        _context.UserPermissions.Remove(userPermission);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByUserAndPermissionAsync(Guid userId, Guid permissionId)
    {
        return await _context.UserPermissions.AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);
    }

    public async Task<(List<UserPermission> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? userId = null, Guid? permissionId = null, bool? isActive = null)
    {
        var query = _context.UserPermissions
            .Include(up => up.User)
            .Include(up => up.Permission)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(up => up.UserId == userId.Value);
        if (permissionId.HasValue)
            query = query.Where(up => up.PermissionId == permissionId.Value);
        if (isActive.HasValue)
            query = query.Where(up => up.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(up => up.AssignedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
