namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Sentiment analysis result for a review
/// </summary>
public class Sentiment
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public string SentimentLabel { get; set; } = string.Empty; // positive|negative|neutral
    public decimal? Confidence { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Review Review { get; set; } = null!;
}
