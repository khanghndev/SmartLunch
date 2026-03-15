namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

public class UserUnitDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid UnitId { get; set; }
    public string? UserName { get; set; }
    public string? UnitName { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}
