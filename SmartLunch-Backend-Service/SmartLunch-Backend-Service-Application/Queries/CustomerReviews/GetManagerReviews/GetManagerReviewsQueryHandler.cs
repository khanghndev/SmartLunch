using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetManagerReviews;

public sealed class GetManagerReviewsQueryHandler : IRequestHandler<GetManagerReviewsQuery, GetManagerReviewsResponse>
{
    private readonly IReviewRepository _reviews;

    public GetManagerReviewsQueryHandler(IReviewRepository reviews) => _reviews = reviews;

    public async Task<GetManagerReviewsResponse> Handle(GetManagerReviewsQuery request, CancellationToken cancellationToken)
    {
        var (items, total, avg) = await _reviews.GetManagerReviewsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.MaxRating,
            cancellationToken);

        return new GetManagerReviewsResponse
        {
            Data = items.Select(ReviewDisplayMapper.ToManagerDto).ToList(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize,
            AverageRating = avg,
        };
    }
}
