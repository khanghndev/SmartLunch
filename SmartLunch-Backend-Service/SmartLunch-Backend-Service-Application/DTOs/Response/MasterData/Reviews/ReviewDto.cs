namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? DishId { get; set; }
    public Guid? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
