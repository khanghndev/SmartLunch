namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;

public class UserTokenDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive { get; set; }
}
