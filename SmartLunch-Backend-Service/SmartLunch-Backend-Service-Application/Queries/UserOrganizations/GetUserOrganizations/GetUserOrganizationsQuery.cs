using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;

namespace SmartLunch.Backend.Service.Application.Queries.UserOrganizations.GetUserOrganizations;

public class GetUserOrganizationsQuery : IRequest<GetUserOrganizationsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? OrganizationId { get; set; }
    public bool? IsActive { get; set; }

    public GetUserOrganizationsQuery(int page, int pageSize, int? userId, int? organizationId, bool? isActive)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        OrganizationId = organizationId;
        IsActive = isActive;
    }
}
