using MediatR;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth;

public class FirebaseLoginCommandHandler : IRequestHandler<FirebaseLoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IFirebaseService _firebaseService;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly int _expireDays;
    private readonly ILogger<FirebaseLoginCommandHandler> _logger;

    public FirebaseLoginCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IFirebaseService firebaseService,
        IJwtService jwtService,
        IConfiguration configuration,
        ILogger<FirebaseLoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _firebaseService = firebaseService;
        _jwtService = jwtService;
        _configuration = configuration;
        _expireDays = int.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(FirebaseLoginCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate input
        if (string.IsNullOrWhiteSpace(req.IdToken))
        {
            throw new ArgumentException("Firebase ID token is required");
        }

        // Verify Firebase ID token
        var firebaseUserInfo = await _firebaseService.VerifyIdTokenAsync(req.IdToken);

        if (string.IsNullOrWhiteSpace(firebaseUserInfo.Email))
        {
            throw new UnauthorizedAccessException("Firebase token does not contain email");
        }

        // Find or create user
        var user = await _userRepository.GetByEmailAsync(firebaseUserInfo.Email);

        if (user == null)
        {
            // Create new user from Firebase info
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = firebaseUserInfo.Email, // Use email as username
                Email = firebaseUserInfo.Email,
                PasswordHash = string.Empty, // No password for social login
                FirstName = firebaseUserInfo.DisplayName?.Split(' ').FirstOrDefault(),
                LastName = firebaseUserInfo.DisplayName?.Split(' ').Skip(1).FirstOrDefault(),
                PhoneNumber = firebaseUserInfo.PhoneNumber,
                Provider = firebaseUserInfo.Provider ?? "firebase",
                IsActive = true,
                IsEmailVerified = firebaseUserInfo.EmailVerified,
                EmailVerifiedAt = firebaseUserInfo.EmailVerified ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            _logger.LogInformation("New user created from Firebase: {Email}", user.Email);
        }
        else
        {
            // Update existing user info if needed
            var firebaseProvider = firebaseUserInfo.Provider ?? "firebase";
            if (user.Provider != firebaseProvider)
            {
                // User exists but provider doesn't match Firebase - update provider
                user.Provider = firebaseProvider;
            }

            // Update email verification status if Firebase says it's verified
            if (firebaseUserInfo.EmailVerified && !user.IsEmailVerified)
            {
                user.IsEmailVerified = true;
                user.EmailVerifiedAt = DateTime.UtcNow;
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive");
        }

        // Get user roles
        var roles = user.UserRoles
            .Where(ur => ur.IsActive)
            .Select(ur => ur.Role.Name)
            .ToList();

        // If user has no roles, assign default "User" role
        if (roles.Count == 0)
        {
            roles.Add("User");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_expireDays);

        // Revoke all existing active tokens for this user
        await _userTokenRepository.RevokeAllUserTokensAsync(user.Id);

        // Store new token in database
        var userToken = new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = refreshTokenExpiresAt,
            IsActive = true
        };

        await _userTokenRepository.CreateAsync(userToken);

        _logger.LogInformation("User logged in successfully via Firebase: {Email}", user.Email);

        return new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
