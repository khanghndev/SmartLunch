namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Units;

public class UnitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
