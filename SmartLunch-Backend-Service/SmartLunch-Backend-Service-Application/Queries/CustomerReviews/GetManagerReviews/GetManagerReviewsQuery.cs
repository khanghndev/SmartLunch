using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetManagerReviews;

public sealed record GetManagerReviewsQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    int? MaxRating) : IRequest<GetManagerReviewsResponse>;
