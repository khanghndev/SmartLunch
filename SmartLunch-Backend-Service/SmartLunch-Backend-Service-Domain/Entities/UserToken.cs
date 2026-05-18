namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// UserPermission junction entity for direct permission assignment to users
/// </summary>
public class UserToken
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = VietnamTime.Now;
    public DateTime ExpiresAt { get; set; } = VietnamTime.Now.AddMinutes(30);
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Jti { get; set; } // JWT ID

    // Navigation property
    public virtual User User { get; set; } = null!;
}