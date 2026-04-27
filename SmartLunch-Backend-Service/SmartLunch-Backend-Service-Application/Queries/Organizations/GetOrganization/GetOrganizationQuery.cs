using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;

namespace SmartLunch.Backend.Service.Application.Queries.Organizations.GetOrganization;

/// <summary>
/// Query to get an organization by ID
/// </summary>
public class GetOrganizationQuery : IRequest<GetOrganizationResponse>
{
    public int OrganizationId { get; set; }

    public GetOrganizationQuery(int organizationId)
    {
        OrganizationId = organizationId;
    }
}
