namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// User-Unit membership (link users to units)
/// </summary>
public class UserUnit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid UnitId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public virtual User User { get; set; } = null!;
    public virtual Unit Unit { get; set; } = null!;
}
