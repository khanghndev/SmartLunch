namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;

public class SentimentDto
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public string SentimentLabel { get; set; } = string.Empty;
    public decimal? Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}
