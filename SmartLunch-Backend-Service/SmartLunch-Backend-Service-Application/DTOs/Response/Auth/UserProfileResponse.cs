namespace SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

public class UserProfileResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Address { get; set; }
    public List<string> Roles { get; set; } = new();

    // Unit info for Organization users
    public UnitInfoResponse? Unit { get; set; }
}

public class UnitInfoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactEmail { get; set; }
    public string UnitType { get; set; } = string.Empty;
}
