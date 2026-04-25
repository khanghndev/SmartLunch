using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UserTokenRepository : IUserTokenRepository
{
    private readonly SmartLunchDBContext _context;

    public UserTokenRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<UserToken?> GetByIdAsync(int id)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .FirstOrDefaultAsync(ut => ut.Id == id);
    }

    public async Task<UserToken?> GetByAccessTokenAsync(string accessToken)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .FirstOrDefaultAsync(ut => ut.AccessToken == accessToken && ut.IsActive);
    }

    public async Task<UserToken?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .FirstOrDefaultAsync(ut => ut.RefreshToken == refreshToken && ut.IsActive);
    }

    public async Task<IEnumerable<UserToken>> GetByUserIdAsync(int userId)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .Where(ut => ut.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserToken>> GetActiveByUserIdAsync(int userId)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .Where(ut => ut.UserId == userId && ut.IsActive)
            .ToListAsync();
    }

    public async Task<(List<UserToken> UserTokens, int TotalCount)> GetUserTokensAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.UserTokens.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(ut => ut.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var userTokens = await query
            .OrderByDescending(ut => ut.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (userTokens, totalCount);
    }

    public async Task<UserToken> CreateAsync(UserToken userToken)
    {
        _context.UserTokens.Add(userToken);
        await _context.SaveChangesAsync();
        return userToken;
    }

    public async Task<UserToken> UpdateAsync(UserToken userToken)
    {
        _context.UserTokens.Update(userToken);
        await _context.SaveChangesAsync();
        return userToken;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userToken = await _context.UserTokens.FindAsync(id);
        if (userToken == null) return false;

        _context.UserTokens.Remove(userToken);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var token = await GetByRefreshTokenAsync(refreshToken);
        if (token == null) return false;

        token.IsActive = false;
        token.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await GetActiveByUserIdAsync(userId);
        foreach (var token in tokens)
        {
            token.IsActive = false;
            token.RevokedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByRefreshTokenAsync(string refreshToken)
    {
        return await _context.UserTokens.AnyAsync(ut => ut.RefreshToken == refreshToken && ut.IsActive);
    }
}
