using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IPasswordHasher passwordHasher,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (string.IsNullOrWhiteSpace(req.CurrentPassword))
            throw new ArgumentException("Current password is required.");
        if (string.IsNullOrWhiteSpace(req.NewPassword))
            throw new ArgumentException("New password is required.");

        if (req.NewPassword.Length < 6)
            throw new ArgumentException("New password must be at least 6 characters.");

        if (!string.Equals(req.NewPassword, req.ConfirmPassword, StringComparison.Ordinal))
            throw new ArgumentException("New password and confirm password do not match.");

        if (string.Equals(req.CurrentPassword, req.NewPassword, StringComparison.Ordinal))
            throw new InvalidOperationException("New password must be different from current password.");

        _logger.LogInformation("Resetting password for user {Email}", req.Email);

        var user = await _userRepository.GetByEmailAsync(req.Email)
            ?? throw new UnauthorizedAccessException("Invalid user context.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is inactive.");

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, req.CurrentPassword))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.HashPassword(req.NewPassword);
        user.UpdatedAt = VietnamTime.Now;
        await _userRepository.UpdateAsync(user);

        await _userTokenRepository.RevokeAllUserTokensAsync(user.Id);

        _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);

        return new ResetPasswordResponse
        {
            UserId = user.Id,
            ChangedAt = VietnamTime.Now
        };
    }
}

