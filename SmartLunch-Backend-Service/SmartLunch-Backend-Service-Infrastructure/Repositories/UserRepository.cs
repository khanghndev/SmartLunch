using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SmartLunchDBContext _context;

    public UserRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserOrganizations)
                .ThenInclude(uo => uo.Organization)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsEmailTakenByAnotherUserAsync(string email, int excludeUserId)
    {
        return await _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeUserId);
    }

    private static readonly HashSet<string> CustomerFacingRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Customer", "Organization", "Company",
        "Khách hàng cá nhân", "Khách hàng doanh nghiệp"
    };

    public async Task<(List<User> Users, int TotalCount)> GetUsersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool? isActive = null,
        string? roleName = null,
        bool? staffOnly = null)
    {
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => 
                u.Username.Contains(searchTerm) || 
                u.Email.Contains(searchTerm) ||
                (u.FirstName != null && u.FirstName.Contains(searchTerm)) ||
                (u.LastName != null && u.LastName.Contains(searchTerm)) ||
                u.Id.ToString() == searchTerm.Trim());
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        if (staffOnly == true)
        {
            query = query.Where(u => u.UserRoles.Any(ur =>
                ur.Role != null && !CustomerFacingRoles.Contains(ur.Role.Name)));
        }

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var rn = roleName.Trim();
            query = query.Where(u => u.UserRoles.Any(ur =>
                ur.Role != null && ur.Role.Name.ToLower() == rn.ToLower()));
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var users = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }
}
