using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiment;

public class GetSentimentQueryHandler : IRequestHandler<GetSentimentQuery, GetSentimentResponse>
{
    private readonly ISentimentRepository _sentimentRepository;
    private readonly ILogger<GetSentimentQueryHandler> _logger;

    public GetSentimentQueryHandler(ISentimentRepository sentimentRepository, ILogger<GetSentimentQueryHandler> logger)
    {
        _sentimentRepository = sentimentRepository;
        _logger = logger;
    }

    public async Task<GetSentimentResponse> Handle(GetSentimentQuery request, CancellationToken cancellationToken)
    {
        var sentiment = await _sentimentRepository.GetByIdAsync(request.SentimentId);

        if (sentiment == null)
        {
            _logger.LogWarning("Sentiment not found with ID: {SentimentId}", request.SentimentId);
            return new GetSentimentResponse { Sentiment = new SentimentDto() };
        }

        return new GetSentimentResponse
        {
            Sentiment = new SentimentDto
            {
                Id = sentiment.Id,
                ReviewId = sentiment.ReviewId,
                SentimentLabel = sentiment.SentimentLabel,
                Confidence = sentiment.Confidence,
                CreatedAt = sentiment.CreatedAt
            }
        };
    }
}
