namespace SmartLunch.Backend.Service.Application.Helpers.Interfaces;

public interface IFirebaseService
{
    Task<FirebaseUserInfo> VerifyIdTokenAsync(string idToken);
}

public class FirebaseUserInfo
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public bool EmailVerified { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Provider { get; set; }
}
