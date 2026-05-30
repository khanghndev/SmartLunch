using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;

namespace SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaints;

public class GetComplaintsQuery : IRequest<GetComplaintsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }

    public GetComplaintsQuery(int page = 1, int pageSize = 10, string? searchTerm = null, string? status = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        Status = status;
    }
}
