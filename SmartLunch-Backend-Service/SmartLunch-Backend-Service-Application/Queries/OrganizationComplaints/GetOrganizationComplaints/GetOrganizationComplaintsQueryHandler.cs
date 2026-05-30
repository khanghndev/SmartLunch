using MediatR;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrganizationComplaints;

public class GetOrganizationComplaintsQueryHandler
    : IRequestHandler<GetOrganizationComplaintsQuery, PaginationResponse<OrganizationComplaintSummaryDto>>
{
    private readonly IComplaintRepository _complaints;

    public GetOrganizationComplaintsQueryHandler(IComplaintRepository complaints) => _complaints = complaints;

    public async Task<PaginationResponse<OrganizationComplaintSummaryDto>> Handle(
        GetOrganizationComplaintsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await _complaints.GetComplaintsAsync(
            request.Page, request.PageSize, searchTerm: null, status: null, userId: request.UserId, cancellationToken);

        return new PaginationResponse<OrganizationComplaintSummaryDto>
        {
            Data = items.Select(ComplaintMapper.ToSummary).ToList(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
