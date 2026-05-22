using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

namespace SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.ReplyToReview;

public sealed record ReplyToReviewCommand(int ReviewId, int ManagerUserId, string Reply) : IRequest<ManagerReviewListItemDto>;
