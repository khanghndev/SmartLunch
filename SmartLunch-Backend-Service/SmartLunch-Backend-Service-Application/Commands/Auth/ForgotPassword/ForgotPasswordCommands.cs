using System.Security.Cryptography;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.Auth.ForgotPassword;

public sealed class PasswordResetTokenPayload
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class RequestForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public ForgotPasswordRequest Request { get; }
    public RequestForgotPasswordCommand(ForgotPasswordRequest request) => Request = request;
}

public class RequestForgotPasswordCommandHandler : IRequestHandler<RequestForgotPasswordCommand, ForgotPasswordResponse>
{
    private const string CachePrefix = "pwd-reset:";
    private static readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);

    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<RequestForgotPasswordCommandHandler> _logger;

    public RequestForgotPasswordCommandHandler(
        IUserRepository userRepository,
        ICacheService cacheService,
        IEmailSender emailSender,
        ILogger<RequestForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> Handle(RequestForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var email = command.Request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user != null && user.IsActive)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            await _cacheService.SetAsync(
                CachePrefix + token,
                new PasswordResetTokenPayload { UserId = user.Id, Email = user.Email },
                TokenTtl,
                cancellationToken);

            var baseUrl = string.IsNullOrWhiteSpace(command.Request.ResetPageUrl)
                ? "/Auth/ResetPassword"
                : command.Request.ResetPageUrl.TrimEnd('/');
            var resetLink = $"{baseUrl}?token={Uri.EscapeDataString(token)}";

            var html = $"""
                <div style="font-family:Segoe UI,sans-serif">
                <h2 style="color:#16a34a">HuitMeal — Đặt lại mật khẩu</h2>
                <p>Xin chào,</p>
                <p>Bạn vừa yêu cầu đặt lại mật khẩu. Nhấn nút bên dưới (hiệu lực 30 phút):</p>
                <p><a href="{resetLink}" style="display:inline-block;background:#f97316;color:#fff;padding:12px 24px;border-radius:8px;text-decoration:none;font-weight:bold">Đặt lại mật khẩu</a></p>
                <p style="color:#64748b;font-size:12px">Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
                <p style="color:#94a3b8;font-size:11px">Link dự phòng: {resetLink}</p>
                </div>
                """;

            try
            {
                await _emailSender.SendAsync(user.Email, "HuitMeal — Đặt lại mật khẩu", html, null, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send password reset email to {Email}", user.Email);
            }

            _logger.LogInformation("Password reset token issued for user {UserId}. Link: {Link}", user.Id, resetLink);
        }

        return new ForgotPasswordResponse
        {
            Message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu."
        };
    }
}

public class ConfirmForgotPasswordCommand : IRequest<ConfirmForgotPasswordResponse>
{
    public ConfirmForgotPasswordRequest Request { get; }
    public ConfirmForgotPasswordCommand(ConfirmForgotPasswordRequest request) => Request = request;
}

public class ConfirmForgotPasswordCommandHandler : IRequestHandler<ConfirmForgotPasswordCommand, ConfirmForgotPasswordResponse>
{
    private const string CachePrefix = "pwd-reset:";

    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICacheService _cacheService;

    public ConfirmForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IPasswordHasher passwordHasher,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _passwordHasher = passwordHasher;
        _cacheService = cacheService;
    }

    public async Task<ConfirmForgotPasswordResponse> Handle(ConfirmForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (string.IsNullOrWhiteSpace(req.Token))
            throw new ArgumentException("Token is required.");
        if (string.IsNullOrWhiteSpace(req.NewPassword))
            throw new ArgumentException("New password is required.");
        if (req.NewPassword.Length < 8)
            throw new ArgumentException("New password must be at least 8 characters.");
        if (!string.Equals(req.NewPassword, req.ConfirmPassword, StringComparison.Ordinal))
            throw new ArgumentException("New password and confirm password do not match.");

        var payload = await _cacheService.GetAsync<PasswordResetTokenPayload>(CachePrefix + req.Token.Trim(), cancellationToken);
        if (payload == null)
            throw new UnauthorizedAccessException("Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

        var user = await _userRepository.GetByIdAsync(payload.UserId)
            ?? throw new UnauthorizedAccessException("Link đặt lại mật khẩu không hợp lệ.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Tài khoản đang bị khóa.");

        user.PasswordHash = _passwordHasher.HashPassword(req.NewPassword);
        user.UpdatedAt = VietnamTime.Now;
        await _userRepository.UpdateAsync(user);
        await _userTokenRepository.RevokeAllUserTokensAsync(user.Id);
        await _cacheService.RemoveAsync(CachePrefix + req.Token.Trim(), cancellationToken);

        return new ConfirmForgotPasswordResponse
        {
            UserId = user.Id,
            ChangedAt = VietnamTime.Now
        };
    }
}
