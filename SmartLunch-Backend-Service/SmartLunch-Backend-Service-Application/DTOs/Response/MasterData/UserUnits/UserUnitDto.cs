namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

public class UserUnitDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UnitId { get; set; }
    public string? UserName { get; set; }
    public string? UnitName { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}
