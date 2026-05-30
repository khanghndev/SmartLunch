using MediatR;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrganizationComplaints;

public record GetOrganizationComplaintsQuery(int UserId, int Page = 1, int PageSize = 20)
    : IRequest<PaginationResponse<OrganizationComplaintSummaryDto>>;
