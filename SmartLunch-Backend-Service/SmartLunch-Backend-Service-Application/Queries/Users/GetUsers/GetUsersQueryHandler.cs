using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Queries.Users.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(IUserRepository userRepository, ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _userRepository.GetUsersAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            request.RoleName,
            request.StaffOnly);

        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            IsActive = u.IsActive,
            IsEmailVerified = u.IsEmailVerified,
            LastLoginAt = u.LastLoginAt,
            CreatedAt = u.CreatedAt,
            RoleNames = u.UserRoles
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role!.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList()
        }).ToList();

        _logger.LogInformation("Retrieved {Count} users (Page {Page}, PageSize {PageSize})",
            userDtos.Count, request.Page, request.PageSize);

        return new GetUsersResponse
        {
            Data = userDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
