namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// User review (dish or order)
/// </summary>
public class Review
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int? DishId { get; set; }
    public int? OrderId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual User User { get; set; } = null!;
    public virtual Dish? Dish { get; set; }
    public virtual Order? Order { get; set; }
    public virtual Sentiment? Sentiment { get; set; }
}
