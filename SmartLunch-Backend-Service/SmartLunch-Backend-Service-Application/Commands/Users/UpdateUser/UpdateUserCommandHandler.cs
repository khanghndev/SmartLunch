using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Users.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, GetUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<GetUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var actor = await _userRepository.GetByIdAsync(request.ActorUserId)
            ?? throw new UnauthorizedAccessException("Actor user not found.");

        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User with ID {request.UserId} was not found.");

        if (TargetIsAdmin(user) && !ActorIsAdmin(actor))
            throw new UnauthorizedAccessException("Only Admin can modify an Admin account.");

        if (request.UserId == request.ActorUserId && req.IsActive == false)
            throw new InvalidOperationException("You cannot lock your own account.");

        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var email = req.Email.Trim();
            if (await _userRepository.IsEmailTakenByAnotherUserAsync(email, user.Id))
                throw new InvalidOperationException("Another user already uses this email.");
            user.Email = email;
        }

        if (req.FirstName != null)
            user.FirstName = string.IsNullOrWhiteSpace(req.FirstName) ? null : req.FirstName.Trim();
        if (req.LastName != null)
            user.LastName = string.IsNullOrWhiteSpace(req.LastName) ? null : req.LastName.Trim();
        if (req.PhoneNumber != null)
            user.PhoneNumber = string.IsNullOrWhiteSpace(req.PhoneNumber) ? null : req.PhoneNumber.Trim();

        if (req.IsActive.HasValue)
            user.IsActive = req.IsActive.Value;

        if (!string.IsNullOrWhiteSpace(req.NewPassword))
        {
            if (req.NewPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters.");
            user.PasswordHash = _passwordHasher.HashPassword(req.NewPassword);
        }

        user.UpdatedAt = VietnamTime.Now;
        await _userRepository.UpdateAsync(user);

        var reloaded = await _userRepository.GetByIdAsync(user.Id);
        if (reloaded == null)
        {
            _logger.LogWarning("User missing after update: {UserId}", user.Id);
            return new GetUserResponse { User = new UserDto() };
        }

        return MapResponse(reloaded);
    }

    private static bool TargetIsAdmin(User user)
    {
        return user.UserRoles.Any(ur =>
            ur.IsActive && ur.Role != null
            && string.Equals(ur.Role.Name, BuiltinRoles.Admin, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ActorIsAdmin(User actor)
    {
        return actor.UserRoles.Any(ur =>
            ur.IsActive && ur.Role != null
            && string.Equals(ur.Role.Name, BuiltinRoles.Admin, StringComparison.OrdinalIgnoreCase));
    }

    private static GetUserResponse MapResponse(User user)
    {
        return new GetUserResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                RoleNames = user.UserRoles
                    .Where(ur => ur.Role != null && ur.IsActive)
                    .Select(ur => ur.Role!.Name)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList()
            }
        };
    }
}
