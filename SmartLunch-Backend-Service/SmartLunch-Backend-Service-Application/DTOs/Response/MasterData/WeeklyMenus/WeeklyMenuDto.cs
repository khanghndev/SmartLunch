namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

public class WeeklyMenuDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
