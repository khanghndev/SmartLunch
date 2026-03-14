using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;

namespace SmartLunch.Backend.Service.Application.Queries.PartnerPayments.GetPartnerPayments;

public class GetPartnerPaymentsQuery : IRequest<GetPartnerPaymentsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetPartnerPaymentsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
