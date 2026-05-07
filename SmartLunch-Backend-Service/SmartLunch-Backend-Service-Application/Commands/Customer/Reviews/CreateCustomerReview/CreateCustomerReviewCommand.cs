using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Customer.Reviews;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

namespace SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.CreateCustomerReview;

public class CreateCustomerReviewCommand : IRequest<CreateCustomerReviewResponse>
{
    public int UserId { get; }
    public CreateCustomerReviewRequest Request { get; }

    public CreateCustomerReviewCommand(int userId, CreateCustomerReviewRequest request)
    {
        UserId = userId;
        Request = request;
    }
}

