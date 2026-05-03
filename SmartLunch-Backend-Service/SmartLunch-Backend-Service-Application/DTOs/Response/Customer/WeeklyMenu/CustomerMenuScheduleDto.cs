namespace SmartLunch.Backend.Service.Application.DTOs.Response.Customer.WeeklyMenu;

public class CustomerMenuScheduleDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public CustomerDishDto Dish { get; set; } = new();
}
