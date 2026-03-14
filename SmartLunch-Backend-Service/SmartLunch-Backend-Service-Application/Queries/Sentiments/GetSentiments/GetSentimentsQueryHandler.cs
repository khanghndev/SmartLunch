using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiments;

public class GetSentimentsQueryHandler : IRequestHandler<GetSentimentsQuery, GetSentimentsResponse>
{
    private readonly ISentimentRepository _sentimentRepository;
    private readonly ILogger<GetSentimentsQueryHandler> _logger;

    public GetSentimentsQueryHandler(ISentimentRepository sentimentRepository, ILogger<GetSentimentsQueryHandler> logger)
    {
        _sentimentRepository = sentimentRepository;
        _logger = logger;
    }

    public async Task<GetSentimentsResponse> Handle(GetSentimentsQuery request, CancellationToken cancellationToken)
    {
        var (sentiments, totalCount) = await _sentimentRepository.GetSentimentsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var sentimentDtos = sentiments.Select(sentiment => new SentimentDto
        {
                Id = sentiment.Id,
                ReviewId = sentiment.ReviewId,
                SentimentLabel = sentiment.SentimentLabel,
                Confidence = sentiment.Confidence,
                CreatedAt = sentiment.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} sentiments (Page {Page}, PageSize {PageSize})",
            sentimentDtos.Count, request.Page, request.PageSize);

        return new GetSentimentsResponse
        {
            Data = sentimentDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
