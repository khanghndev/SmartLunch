using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetPublicReviews;

public sealed record GetPublicReviewsQuery(int Page = 1, int PageSize = 50) : IRequest<GetPublicReviewsResponse>;
