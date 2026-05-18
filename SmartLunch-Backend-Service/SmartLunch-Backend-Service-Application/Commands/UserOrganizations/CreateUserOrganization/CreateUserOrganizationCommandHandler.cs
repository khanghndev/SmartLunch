using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.UserOrganizations.CreateUserOrganization;

public class CreateUserOrganizationCommandHandler : IRequestHandler<CreateUserOrganizationCommand, CreateUserOrganizationResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public CreateUserOrganizationCommandHandler(
        IUserOrganizationRepository userOrganizationRepository,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository)
    {
        _userOrganizationRepository = userOrganizationRepository;
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<CreateUserOrganizationResponse> Handle(CreateUserOrganizationCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (await _userRepository.GetByIdAsync(req.UserId) == null)
            throw new KeyNotFoundException($"User with ID {req.UserId} not found");
        if (await _organizationRepository.GetByIdAsync(req.OrganizationId) == null)
            throw new KeyNotFoundException($"Organization with ID {req.OrganizationId} not found");
        if (await _userOrganizationRepository.ExistsByUserAndOrganizationAsync(req.UserId, req.OrganizationId))
            throw new InvalidOperationException("User is already assigned to this organization");

        var entity = new UserOrganization
        {

            UserId = req.UserId,
            OrganizationId = req.OrganizationId,
            JoinedAt = VietnamTime.Now,
            IsActive = true
        };
        await _userOrganizationRepository.CreateAsync(entity);

        return new CreateUserOrganizationResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            OrganizationId = entity.OrganizationId,
            Message = "User organization created successfully"
        };
    }
}
