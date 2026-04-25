namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;

public class ReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? DishId { get; set; }
    public int? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
