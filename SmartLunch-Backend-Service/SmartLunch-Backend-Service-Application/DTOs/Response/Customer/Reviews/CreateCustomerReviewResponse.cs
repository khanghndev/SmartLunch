using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

public class CreateCustomerReviewResponse
{
    public ReviewDto Review { get; set; } = new();
}

