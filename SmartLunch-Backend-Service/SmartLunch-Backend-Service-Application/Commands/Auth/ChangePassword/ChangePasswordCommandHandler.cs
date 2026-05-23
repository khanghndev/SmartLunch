using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.Auth.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResetPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IPasswordHasher passwordHasher,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<ResetPasswordResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (string.IsNullOrWhiteSpace(req.CurrentPassword))
            throw new ArgumentException("Current password is required.");
        if (string.IsNullOrWhiteSpace(req.NewPassword))
            throw new ArgumentException("New password is required.");
        if (req.NewPassword.Length < 8)
            throw new ArgumentException("New password must be at least 8 characters.");
        if (!string.Equals(req.NewPassword, req.ConfirmPassword, StringComparison.Ordinal))
            throw new ArgumentException("New password and confirm password do not match.");
        if (string.Equals(req.CurrentPassword, req.NewPassword, StringComparison.Ordinal))
            throw new InvalidOperationException("New password must be different from current password.");

        var user = await _userRepository.GetByIdAsync(command.UserId)
            ?? throw new UnauthorizedAccessException("Invalid user context.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is inactive.");

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, req.CurrentPassword))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.HashPassword(req.NewPassword);
        user.UpdatedAt = VietnamTime.Now;
        await _userRepository.UpdateAsync(user);

        await _userTokenRepository.RevokeAllUserTokensAsync(user.Id);

        _logger.LogInformation("Password changed for user {UserId}", user.Id);

        return new ResetPasswordResponse
        {
            UserId = user.Id,
            ChangedAt = VietnamTime.Now
        };
    }
}
