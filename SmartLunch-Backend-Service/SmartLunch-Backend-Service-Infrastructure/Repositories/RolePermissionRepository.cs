using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly SmartLunchDBContext _context;

    public RolePermissionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<RolePermission?> GetByIdAsync(Guid id)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .FirstOrDefaultAsync(rp => rp.Id == id);
    }

    public async Task<RolePermission?> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }

    public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RolePermission>> GetActiveByRoleIdAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId && rp.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(Guid permissionId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.PermissionId == permissionId)
            .ToListAsync();
    }

    public async Task<RolePermission> CreateAsync(RolePermission rolePermission)
    {
        _context.RolePermissions.Add(rolePermission);
        await _context.SaveChangesAsync();
        return rolePermission;
    }

    public async Task<RolePermission> UpdateAsync(RolePermission rolePermission)
    {
        _context.RolePermissions.Update(rolePermission);
        await _context.SaveChangesAsync();
        return rolePermission;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var rolePermission = await _context.RolePermissions.FindAsync(id);
        if (rolePermission == null) return false;

        _context.RolePermissions.Remove(rolePermission);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByRoleAndPermissionAsync(Guid roleId, Guid permissionId)
    {
        var rolePermission = await GetByRoleAndPermissionAsync(roleId, permissionId);
        if (rolePermission == null) return false;

        _context.RolePermissions.Remove(rolePermission);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByRoleAndPermissionAsync(Guid roleId, Guid permissionId)
    {
        return await _context.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }
}
