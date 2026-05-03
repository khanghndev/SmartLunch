namespace SmartLunch.Backend.Service.Application.DTOs.Response.Customer.WeeklyMenu;

public class CustomerWeeklyMenuDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public List<CustomerMenuScheduleDto> Schedules { get; set; } = new();
}