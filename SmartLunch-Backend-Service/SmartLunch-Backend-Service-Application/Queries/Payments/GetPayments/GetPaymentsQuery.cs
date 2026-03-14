using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Payments;

namespace SmartLunch.Backend.Service.Application.Queries.Payments.GetPayments;

public class GetPaymentsQuery : IRequest<GetPaymentsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetPaymentsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
