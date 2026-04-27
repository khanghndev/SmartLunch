using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserOrganizations.GetUserOrganization;

public class GetUserOrganizationQueryHandler : IRequestHandler<GetUserOrganizationQuery, GetUserOrganizationResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public GetUserOrganizationQueryHandler(IUserOrganizationRepository userOrganizationRepository)
    {
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<GetUserOrganizationResponse> Handle(GetUserOrganizationQuery request, CancellationToken cancellationToken)
    {
        var uu = await _userOrganizationRepository.GetByIdAsync(request.Id);
        if (uu == null)
            return new GetUserOrganizationResponse { UserOrganization = new UserOrganizationDto() };

        return new GetUserOrganizationResponse
        {
            UserOrganization = new UserOrganizationDto
            {
                Id = uu.Id,
                UserId = uu.UserId,
                OrganizationId = uu.OrganizationId,
                UserName = uu.User?.Username,
                OrganizationName = uu.Organization?.Name,
                JoinedAt = uu.JoinedAt,
                IsActive = uu.IsActive
            }
        };
    }
}
