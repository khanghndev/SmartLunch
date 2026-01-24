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

    public async Task<UserRole?> GetByIdAsync(Guid id)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.Id == id);
    }

    public async Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId)
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

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);
        if (userRole == null) return false;

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByUserAndRoleAsync(Guid userId, Guid roleId)
    {
        var userRole = await GetByUserAndRoleAsync(userId, roleId);
        if (userRole == null) return false;

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId)
    {
        return await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }
}
