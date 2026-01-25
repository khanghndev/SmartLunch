using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Queries.Users;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
            return new GetUserResponse { User = new UserDto() };
        }

        // Map to UserDto (assumes some mapping logic, either manual or via, e.g., AutoMapper)
        var response = new GetUserResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            }
        };

        _logger.LogInformation("Retrieved user with ID: {UserId}", request.UserId);

        return response;
    }
}
