namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// User review (dish or order)
/// </summary>
public class Review
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? DishId { get; set; }
    public Guid? OrderId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Dish? Dish { get; set; }
    public virtual Order? Order { get; set; }
    public virtual Sentiment? Sentiment { get; set; }
}
