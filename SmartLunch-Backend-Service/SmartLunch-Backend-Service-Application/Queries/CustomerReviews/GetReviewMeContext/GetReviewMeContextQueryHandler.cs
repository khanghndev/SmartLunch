using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetReviewMeContext;

public sealed class GetReviewMeContextQueryHandler : IRequestHandler<GetReviewMeContextQuery, GetReviewMeContextResponse>
{
    private readonly IReviewRepository _reviews;
    private readonly IUserOrganizationRepository _userOrganizations;

    public GetReviewMeContextQueryHandler(
        IReviewRepository reviews,
        IUserOrganizationRepository userOrganizations)
    {
        _reviews = reviews;
        _userOrganizations = userOrganizations;
    }

    public async Task<GetReviewMeContextResponse> Handle(GetReviewMeContextQuery request, CancellationToken cancellationToken)
    {
        var orgIds = await OrganizationReviewRules.GetActiveOrganizationIdsAsync(
            _userOrganizations, request.UserId, cancellationToken);

        if (orgIds.Count == 0)
        {
            return new GetReviewMeContextResponse
            {
                IsEnterpriseMember = false,
                CanSubmitReview = false,
                Message = "Chỉ khách hàng doanh nghiệp đã đặt suất ăn mới có thể gửi đánh giá. Bạn có thể xem ý kiến của các đối tác khác.",
            };
        }

        var orders = await _reviews.GetEnterpriseOrdersForUserAsync(request.UserId, orgIds, cancellationToken);
        if (orders.Count == 0)
        {
            return new GetReviewMeContextResponse
            {
                IsEnterpriseMember = true,
                CanSubmitReview = false,
                Message = "Đơn vị của bạn chưa có đơn đặt suất ăn. Sau khi đặt và hoàn tất đơn, bạn có thể đánh giá tại đây.",
            };
        }

        var reviewable = new List<ReviewableOrderDto>();
        var canSubmit = false;

        foreach (var order in orders)
        {
            var already = await _reviews.ExistsForUserAsync(request.UserId, null, order.Id, cancellationToken);
            var eligible = OrganizationReviewRules.IsReviewableOrderStatus(order);
            if (eligible && !already)
                canSubmit = true;

            reviewable.Add(new ReviewableOrderDto
            {
                OrderId = order.Id,
                OrderCode = order.Code,
                InvoiceCode = order.InvoiceCode,
                ScheduledDate = order.ScheduledDate,
                Status = order.Status,
                OrganizationName = order.Contract?.Organization?.Name,
                AlreadyReviewed = already,
            });
        }

        return new GetReviewMeContextResponse
        {
            IsEnterpriseMember = true,
            CanSubmitReview = canSubmit,
            Message = canSubmit
                ? null
                : "Bạn đã đánh giá các đơn đủ điều kiện, hoặc đơn chưa ở trạng thái hoàn tất (đã giao / đã thanh toán).",
            ReviewableOrders = reviewable.Where(o => !o.AlreadyReviewed).ToList(),
        };
    }
}
