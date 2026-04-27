using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.UserOrganizations.UpdateUserOrganization;

public class UpdateUserOrganizationCommandHandler : IRequestHandler<UpdateUserOrganizationCommand, GetUserOrganizationResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public UpdateUserOrganizationCommandHandler(IUserOrganizationRepository userOrganizationRepository)
    {
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<GetUserOrganizationResponse> Handle(UpdateUserOrganizationCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _userOrganizationRepository.GetByIdAsync(req.Id);
        if (entity == null)
            throw new KeyNotFoundException($"UserOrganization with ID {req.Id} not found");

        entity.IsActive = req.IsActive;
        await _userOrganizationRepository.UpdateAsync(entity);

        return new GetUserOrganizationResponse
        {
            UserOrganization = new UserOrganizationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OrganizationId = entity.OrganizationId,
                UserName = entity.User?.Username,
                OrganizationName = entity.Organization?.Name,
                JoinedAt = entity.JoinedAt,
                IsActive = entity.IsActive
            }
        };
    }
}
