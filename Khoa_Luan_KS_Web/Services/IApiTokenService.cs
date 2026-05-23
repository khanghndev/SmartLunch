namespace Khoa_Luan_KS_Web.Services;

/// <summary>
/// Quản lý access/refresh token trong Session + cookie cho mọi khu vực (Admin, Manager, WarehouseStaff, Customer).
/// </summary>
public interface IApiTokenService
{
    string? GetAccessToken();
    string? GetRefreshToken();

    void PersistTokens(string accessToken, string refreshToken, string? email = null, string? displayName = null);
    void ClearTokens();

    /// <summary>Trả access token hợp lệ; tự gọi refresh nếu sắp hết hạn hoặc đã hết hạn.</summary>
    Task<string?> EnsureValidAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>Luôn thử refresh (dùng khi API trả 401).</summary>
    Task<string?> ForceRefreshAsync(CancellationToken cancellationToken = default);

    bool IsAccessTokenExpiringSoon(string? accessToken);
}
