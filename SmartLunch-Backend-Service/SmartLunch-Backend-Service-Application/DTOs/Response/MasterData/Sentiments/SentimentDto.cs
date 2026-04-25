namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;

public class SentimentDto
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public string SentimentLabel { get; set; } = string.Empty;
    public decimal? Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}
