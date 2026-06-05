namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Một dòng món chính trong suất ăn tuần theo hợp đồng.</summary>
public class ContractWeeklySelectionItem
{
    public int Id { get; set; }
    public int WeeklySelectionId { get; set; }
    public DateOnly ServiceDate { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; }

    public virtual ContractWeeklySelection WeeklySelection { get; set; } = null!;
    public virtual Dish Dish { get; set; } = null!;
}
