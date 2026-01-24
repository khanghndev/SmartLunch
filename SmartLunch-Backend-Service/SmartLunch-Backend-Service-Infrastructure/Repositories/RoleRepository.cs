using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly SmartLunchDBContext _context;

    public RoleRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name == name && r.IsActive);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync()
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        // Check if it's a system role
        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("Cannot delete system role");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Roles.AnyAsync(r => r.Name == name);
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _context.Roles.AnyAsync(r => r.Id == id);
    }
}
