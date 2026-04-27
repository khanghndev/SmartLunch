using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UserOrganizationRepository : IUserOrganizationRepository
{
    private readonly SmartLunchDBContext _context;

    public UserOrganizationRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<UserOrganization?> GetByIdAsync(int id)
    {
        return await _context.UserOrganizations
            .Include(uu => uu.User)
            .Include(uu => uu.Organization)
            .FirstOrDefaultAsync(uu => uu.Id == id);
    }

    public async Task<UserOrganization?> GetByUserAndOrganizationAsync(int userId, int organizationId)
    {
        return await _context.UserOrganizations
            .Include(uu => uu.User)
            .Include(uu => uu.Organization)
            .FirstOrDefaultAsync(uu => uu.UserId == userId && uu.OrganizationId == organizationId);
    }

    public async Task<IEnumerable<UserOrganization>> GetByUserIdAsync(int userId)
    {
        return await _context.UserOrganizations
            .Include(uu => uu.User)
            .Include(uu => uu.Organization)
            .Where(uu => uu.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserOrganization>> GetActiveByUserIdAsync(int userId)
    {
        return await _context.UserOrganizations
            .Include(uu => uu.User)
            .Include(uu => uu.Organization)
            .Where(uu => uu.UserId == userId && uu.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserOrganization>> GetByOrganizationIdAsync(int organizationId)
    {
        return await _context.UserOrganizations
            .Include(uu => uu.User)
            .Include(uu => uu.Organization)
            .Where(uu => uu.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<(List<UserOrganization> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? organizationId = null, bool? isActive = null)
    {
        var query = _context.UserOrganizations
            .Include(uu => uu.User)
            .Include(uu => uu.Organization)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(uu => uu.UserId == userId.Value);
        if (organizationId.HasValue)
            query = query.Where(uu => uu.OrganizationId == organizationId.Value);
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

    public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
    {
        _context.UserOrganizations.Add(userOrganization);
        await _context.SaveChangesAsync();
        return userOrganization;
    }

    public async Task<UserOrganization> UpdateAsync(UserOrganization userOrganization)
    {
        _context.UserOrganizations.Update(userOrganization);
        await _context.SaveChangesAsync();
        return userOrganization;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userOrganization = await _context.UserOrganizations.FindAsync(id);
        if (userOrganization == null) return false;

        _context.UserOrganizations.Remove(userOrganization);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByUserAndOrganizationAsync(int userId, int organizationId)
    {
        var userOrganization = await GetByUserAndOrganizationAsync(userId, organizationId);
        if (userOrganization == null) return false;

        _context.UserOrganizations.Remove(userOrganization);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByUserAndOrganizationAsync(int userId, int organizationId)
    {
        return await _context.UserOrganizations.AnyAsync(uu => uu.UserId == userId && uu.OrganizationId == organizationId);
    }
}
