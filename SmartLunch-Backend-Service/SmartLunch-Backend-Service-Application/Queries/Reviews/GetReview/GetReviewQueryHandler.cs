using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Reviews.GetReview;

public class GetReviewQueryHandler : IRequestHandler<GetReviewQuery, GetReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<GetReviewQueryHandler> _logger;

    public GetReviewQueryHandler(IReviewRepository reviewRepository, ILogger<GetReviewQueryHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<GetReviewResponse> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId);

        if (review == null)
        {
            _logger.LogWarning("Review not found with ID: {ReviewId}", request.ReviewId);
            return new GetReviewResponse { Review = new ReviewDto() };
        }

        return new GetReviewResponse
        {
            Review = new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                DishId = review.DishId,
                OrderId = review.OrderId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            }
        };
    }
}
