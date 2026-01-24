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

    public async Task<UserToken?> GetByIdAsync(Guid id)
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

    public async Task<IEnumerable<UserToken>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .Where(ut => ut.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.UserTokens
            .Include(ut => ut.User)
            .Where(ut => ut.UserId == userId && ut.IsActive)
            .ToListAsync();
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

    public async Task<bool> DeleteAsync(Guid id)
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

    public async Task<bool> RevokeAllUserTokensAsync(Guid userId)
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
