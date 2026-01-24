using MediatR;
using SmartLunch.Backend.Service.Application.Queries.Auth;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<string>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetUserRolesQueryHandler> _logger;

    public GetUserRolesQueryHandler(
        IUserRoleRepository userRoleRepository,
        ILogger<GetUserRolesQueryHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<List<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetByUserIdAsync(request.UserId);

        _logger.LogInformation("Retrieved {Count} roles for user: {UserId}", userRoles.Count(), request.UserId);

        return userRoles.Select(ur => ur.Role.Name).ToList();
    }
}
