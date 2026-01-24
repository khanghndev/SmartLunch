using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly SmartLunchDBContext _context;

    public PermissionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .Include(p => p.UserPermissions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .Include(p => p.UserPermissions)
            .FirstOrDefaultAsync(p => p.Name == name && p.IsActive);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .Include(p => p.UserPermissions)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .Include(p => p.UserPermissions)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetByResourceAsync(string resource)
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .Include(p => p.UserPermissions)
            .Where(p => p.Resource == resource && p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetByActionAsync(string action)
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .Include(p => p.UserPermissions)
            .Where(p => p.Action == action && p.IsActive)
            .ToListAsync();
    }

    public async Task<Permission> CreateAsync(Permission permission)
    {
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<Permission> UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null) return false;

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Permissions.AnyAsync(p => p.Name == name);
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _context.Permissions.AnyAsync(p => p.Id == id);
    }
}
