using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationReviews;

public static class ReviewDisplayMapper
{
    public static string GetUserDisplayName(User? user)
    {
        if (user == null) return "Khách hàng";
        var full = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(full) ? user.Username : full;
    }

    public static PublicReviewDto ToPublicDto(Review review) => new()
    {
        Id = review.Id,
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt,
        AuthorName = GetUserDisplayName(review.User),
        OrganizationName = review.Order?.Contract?.Organization?.Name,
        OrderCode = review.Order?.Code ?? review.Order?.InvoiceCode,
        ManagerReply = review.ManagerReply,
        RepliedAt = review.RepliedAt,
    };

    public static ManagerReviewListItemDto ToManagerDto(Review review) => new()
    {
        Id = review.Id,
        UserId = review.UserId,
        OrderId = review.OrderId,
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt,
        CustomerName = GetUserDisplayName(review.User),
        OrganizationName = review.Order?.Contract?.Organization?.Name,
        OrderCode = review.Order?.Code,
        InvoiceCode = review.Order?.InvoiceCode,
        ManagerReply = review.ManagerReply,
        RepliedAt = review.RepliedAt,
    };
}
