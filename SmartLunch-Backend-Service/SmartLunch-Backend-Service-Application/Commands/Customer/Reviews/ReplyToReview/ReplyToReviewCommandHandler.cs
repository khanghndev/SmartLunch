using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;

namespace SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.ReplyToReview;

public sealed class ReplyToReviewCommandHandler : IRequestHandler<ReplyToReviewCommand, ManagerReviewListItemDto>
{
    private readonly IReviewRepository _reviews;

    public ReplyToReviewCommandHandler(IReviewRepository reviews) => _reviews = reviews;

    public async Task<ManagerReviewListItemDto> Handle(ReplyToReviewCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reply))
            throw new ArgumentException("Reply content is required.");

        var entity = await _reviews.GetByIdWithDetailsAsync(request.ReviewId, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException($"Review not found with ID: {request.ReviewId}");

        entity.ManagerReply = request.Reply.Trim();
        entity.RepliedAt = VietnamTime.Now;
        entity.RepliedByUserId = request.ManagerUserId;

        await _reviews.UpdateAsync(entity, cancellationToken);

        return ReviewDisplayMapper.ToManagerDto(entity);
    }
}
