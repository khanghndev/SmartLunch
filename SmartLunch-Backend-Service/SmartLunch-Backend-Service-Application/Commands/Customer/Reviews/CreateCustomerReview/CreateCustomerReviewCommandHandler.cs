using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.CreateCustomerReview;

public class CreateCustomerReviewCommandHandler : IRequestHandler<CreateCustomerReviewCommand, CreateCustomerReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IOrderRepository _orderRepository;

    public CreateCustomerReviewCommandHandler(
        IReviewRepository reviewRepository,
        IDishRepository dishRepository,
        IOrderRepository orderRepository)
    {
        _reviewRepository = reviewRepository;
        _dishRepository = dishRepository;
        _orderRepository = orderRepository;
    }

    public async Task<CreateCustomerReviewResponse> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var req = request.Request;

        if (req.DishId == null && req.OrderId == null)
            throw new ArgumentException("DishId or OrderId is required.");
        if (req.DishId != null && req.OrderId != null)
            throw new ArgumentException("Only one of DishId or OrderId can be provided.");
        if (req.Rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        var dishId = req.DishId;
        var orderId = req.OrderId;

        var exists = await _reviewRepository.ExistsForUserAsync(userId, dishId, orderId, cancellationToken);
        if (exists)
            throw new InvalidOperationException("You have already reviewed this item.");

        if (dishId != null)
        {
            var dish = await _dishRepository.GetByIdAsync(dishId.Value);
            if (dish == null)
                throw new KeyNotFoundException($"Dish not found with ID: {dishId.Value}");
        }

        if (orderId != null)
        {
            var order = await _orderRepository.GetByIdAsync(orderId.Value);
            if (order == null)
                throw new KeyNotFoundException($"Order not found with ID: {orderId.Value}");

            if (order.UserId == null || order.UserId.Value != userId)
                throw new UnauthorizedAccessException("You do not have access to review this order.");

            if (!string.Equals(order.Status, "delivered", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only delivered orders can be reviewed.");
        }

        var entity = new Review
        {
            UserId = userId,
            DishId = dishId,
            OrderId = orderId,
            Rating = req.Rating,
            Comment = string.IsNullOrWhiteSpace(req.Comment) ? null : req.Comment.Trim(),
            CreatedAt = VietnamTime.Now
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
                CreatedAt = created.CreatedAt
            }
        };
    }
}

