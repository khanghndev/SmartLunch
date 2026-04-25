using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserTokenRepository
{
    Task<UserToken?> GetByIdAsync(int id);
    Task<UserToken?> GetByAccessTokenAsync(string accessToken);
    Task<UserToken?> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<UserToken>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserToken>> GetActiveByUserIdAsync(int userId);
    Task<(List<UserToken> UserTokens, int TotalCount)> GetUserTokensAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
    Task<UserToken> CreateAsync(UserToken userToken);
    Task<UserToken> UpdateAsync(UserToken userToken);
    Task<bool> DeleteAsync(int id);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> RevokeAllUserTokensAsync(int userId);
    Task<bool> ExistsByRefreshTokenAsync(string refreshToken);
}
