namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// User-Unit membership (link users to units)
/// </summary>
public class UserOrganization
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public virtual User User { get; set; } = null!;
    public virtual Organization Organization { get; set; } = null!;
}
