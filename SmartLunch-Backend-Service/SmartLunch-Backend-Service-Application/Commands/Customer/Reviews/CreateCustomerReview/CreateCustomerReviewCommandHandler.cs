using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.CreateCustomerReview;

public class CreateCustomerReviewCommandHandler : IRequestHandler<CreateCustomerReviewCommand, CreateCustomerReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public CreateCustomerReviewCommandHandler(
        IReviewRepository reviewRepository,
        IOrderRepository orderRepository,
        IUserOrganizationRepository userOrganizationRepository)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<CreateCustomerReviewResponse> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var req = request.Request;

        if (req.OrderId == null)
            throw new ArgumentException("Chỉ được đánh giá theo đơn hàng suất ăn doanh nghiệp (OrderId bắt buộc).");
        if (req.DishId != null)
            throw new ArgumentException("Đánh giá theo món riêng lẻ không áp dụng cho khách doanh nghiệp.");
        if (req.Rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        var orgIds = await OrganizationReviewRules.GetActiveOrganizationIdsAsync(
            _userOrganizationRepository, userId, cancellationToken);
        if (orgIds.Count == 0)
            throw new UnauthorizedAccessException("Chỉ khách hàng doanh nghiệp đã liên kết đơn vị mới được gửi đánh giá.");

        var orderId = req.OrderId.Value;
        var exists = await _reviewRepository.ExistsForUserAsync(userId, null, orderId, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Bạn đã đánh giá đơn hàng này.");

        var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
            throw new KeyNotFoundException($"Order not found with ID: {orderId}");

        if (!OrganizationReviewRules.UserCanAccessOrder(order, userId, orgIds))
            throw new UnauthorizedAccessException("Bạn không có quyền đánh giá đơn hàng này.");

        if (!OrganizationReviewRules.IsReviewableOrderStatus(order))
            throw new InvalidOperationException("Chỉ đánh giá được khi đơn đã giao hoặc đã xác nhận và thanh toán.");

        var entity = new Review
        {
            UserId = userId,
            OrderId = orderId,
            Rating = req.Rating,
            Comment = string.IsNullOrWhiteSpace(req.Comment) ? null : req.Comment.Trim(),
            CreatedAt = VietnamTime.Now,
        };

        var created = await _reviewRepository.CreateAsync(entity, cancellationToken);

        return new CreateCustomerReviewResponse
        {
            Review = new ReviewDto
            {
                Id = created.Id,
                UserId = created.UserId,
                DishId = created.DishId,
                OrderId = created.OrderId,
                Rating = created.Rating,
                Comment = created.Comment,
                CreatedAt = created.CreatedAt,
            },
        };
    }
}
