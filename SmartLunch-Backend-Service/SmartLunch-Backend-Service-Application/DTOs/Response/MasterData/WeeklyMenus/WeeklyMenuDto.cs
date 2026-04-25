namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

public class WeeklyMenuDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
