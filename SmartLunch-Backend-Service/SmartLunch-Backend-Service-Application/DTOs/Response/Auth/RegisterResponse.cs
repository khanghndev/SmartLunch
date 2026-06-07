namespace SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

public class RegisterResponse
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Organization";
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
}
