using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserOrganizations.GetUserOrganizations;

public class GetUserOrganizationsQueryHandler : IRequestHandler<GetUserOrganizationsQuery, GetUserOrganizationsResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public GetUserOrganizationsQueryHandler(IUserOrganizationRepository userOrganizationRepository)
    {
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<GetUserOrganizationsResponse> Handle(GetUserOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userOrganizationRepository.GetPagedAsync(
            request.Page, request.PageSize, request.UserId, request.OrganizationId, request.IsActive);

        var dtos = items.Select(uu => new UserOrganizationDto
        {
            Id = uu.Id,
            UserId = uu.UserId,
            OrganizationId = uu.OrganizationId,
            UserName = uu.User?.Username,
            OrganizationName = uu.Organization?.Name,
            JoinedAt = uu.JoinedAt,
            IsActive = uu.IsActive
        }).ToList();

        return new GetUserOrganizationsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
