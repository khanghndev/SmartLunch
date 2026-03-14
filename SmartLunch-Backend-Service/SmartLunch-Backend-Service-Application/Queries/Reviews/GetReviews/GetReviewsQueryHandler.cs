using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Reviews.GetReviews;

public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, GetReviewsResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<GetReviewsQueryHandler> _logger;

    public GetReviewsQueryHandler(IReviewRepository reviewRepository, ILogger<GetReviewsQueryHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<GetReviewsResponse> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var (reviews, totalCount) = await _reviewRepository.GetReviewsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var reviewDtos = reviews.Select(review => new ReviewDto
        {
                Id = review.Id,
                UserId = review.UserId,
                DishId = review.DishId,
                OrderId = review.OrderId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} reviews (Page {Page}, PageSize {PageSize})",
            reviewDtos.Count, request.Page, request.PageSize);

        return new GetReviewsResponse
        {
            Data = reviewDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
