namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Số suất ăn tùy chỉnh cho một ngày phục vụ (khác mặc định MealsPerDay).</summary>
public class ContractDailyMealPortion
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public DateOnly ServiceDate { get; set; }
    public int MealCount { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Contract Contract { get; set; } = null!;
}
