using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetReviewMeContext;

public sealed record GetReviewMeContextQuery(int UserId) : IRequest<GetReviewMeContextResponse>;
