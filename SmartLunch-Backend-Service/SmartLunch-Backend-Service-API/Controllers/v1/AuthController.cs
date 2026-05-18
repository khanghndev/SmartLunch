using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using System.Net;
using System.Security.Claims;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.AspNetCore.RateLimiting;
using FirebaseLoginRequest = SmartLunch.Backend.Service.Application.DTOs.Request.Auth.FirebaseLoginRequest;
using FirebaseLoginCommand = SmartLunch.Backend.Service.Application.Commands.Auth.FirebaseLoginCommand;
using SmartLunch.Backend.Service.Application.Commands.Auth.LoginAdmin;
using SmartLunch.Backend.Service.Application.Commands.Auth.LoginUser;
using SmartLunch.Backend.Service.Application.Queries.Auth;

using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.API.Controllers
{
    /// <summary>
    /// Authentication controller for user login, registration, and token management
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;
        private readonly SmartLunchDBContext _db;
        private readonly IStorageService _storage;
        private readonly IMediaFileRepository _mediaFiles;
        private readonly IConfiguration _configuration;

        public AuthController(
            ILogger<AuthController> logger,
            IMediator mediator,
            SmartLunchDBContext db,
            IStorageService storage,
            IMediaFileRepository mediaFiles,
            IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _db = db;
            _storage = storage;
            _mediaFiles = mediaFiles;
            _configuration = configuration;
        }

        [HttpPost("login-admin")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> LoginAdmin(LoginAdminRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LoginAdminCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> LoginUser(LoginUserRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LoginUserCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("firebase-login")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> FirebaseLogin([FromBody] FirebaseLoginRequest request)
        {
            // Check model validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogWarning("Firebase login validation failed: {Errors}", string.Join(", ", errors));
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult("Invalid request", errors));
            }

            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
            {
                _logger.LogWarning("Firebase login request is null or IdToken is empty");
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult("IdToken is required", new[] { "IdToken is required" }));
            }

            try
            {
                var response = await _mediator.Send(new FirebaseLoginCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Firebase login successful"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Firebase login argument exception");
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Firebase login unauthorized");
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Firebase login invalid operation");
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseApiResponse<RegisterResponse>>> Register(RegisterRequest request)
        {
            try
            {
                var response = await _mediator.Send(new RegisterCommand(request));
                return Ok(BaseApiResponse<RegisterResponse>.SuccessResult(response, "User registered successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<RegisterResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseApiResponse<RegisterResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<LogoutResponse>>> Logout(LogoutRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LogoutCommand(request));
                return Ok(BaseApiResponse<LogoutResponse>.SuccessResult(response, "Logout successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LogoutResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LogoutResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<RefreshTokenResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var response = await _mediator.Send(new RefreshTokenCommand(request));
                return Ok(BaseApiResponse<RefreshTokenResponse>.SuccessResult(response, "Token refreshed successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<RefreshTokenResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<RefreshTokenResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseApiResponse<RefreshTokenResponse>.NotFoundResult(ex.Message));
            }
        }

        [HttpPut("reset-password")]
        public async Task<ActionResult<BaseApiResponse<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(BaseApiResponse<ResetPasswordResponse>.ErrorResult("Invalid request", errors));
            }

            try
            {
                var response = await _mediator.Send(new ResetPasswordCommand(request));
                return Ok(BaseApiResponse<ResetPasswordResponse>.SuccessResult(response, "Password reset successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<ResetPasswordResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseApiResponse<ResetPasswordResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<ResetPasswordResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseApiResponse<ResetPasswordResponse>.NotFoundResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    BaseApiResponse<ResetPasswordResponse>.ErrorResult("An error occurred while resetting password", new[] { ex.Message }));
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<UserProfileResponse>>> GetProfile()
        {
            try
            {
                var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
                    throw new UnauthorizedAccessException("Invalid user context.");

                var response = await _mediator.Send(new GetProfileQuery(userId));
                return Ok(BaseApiResponse<UserProfileResponse>.SuccessResult(response, "Profile retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseApiResponse<UserProfileResponse>.NotFoundResult(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<UserProfileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile");
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    BaseApiResponse<UserProfileResponse>.ErrorResult("An error occurred while retrieving profile", new[] { ex.Message }));
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<UserProfileResponse>>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
                    throw new UnauthorizedAccessException("Invalid user context.");

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return NotFound(BaseApiResponse<UserProfileResponse>.NotFoundResult("User not found"));

                user.FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? null : request.FirstName.Trim();
                user.LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim();
                user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
                user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
                user.UpdatedAt = VietnamTime.Now;

                await _db.SaveChangesAsync();

                var response = await _mediator.Send(new GetProfileQuery(userId));
                return Ok(BaseApiResponse<UserProfileResponse>.SuccessResult(response, "Profile updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<UserProfileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    BaseApiResponse<UserProfileResponse>.ErrorResult("An error occurred while updating profile", new[] { ex.Message }));
            }
        }

        [HttpPost("profile/avatar")]
        [Authorize]
        [EnableRateLimiting("media-upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<BaseApiResponse<UploadAvatarResponse>>> UploadAvatar([FromForm] IFormFile file)
        {
            try
            {
                var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
                    throw new UnauthorizedAccessException("Invalid user context.");

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return NotFound(BaseApiResponse<UploadAvatarResponse>.NotFoundResult("User not found"));

                if (file == null || file.Length <= 0)
                    return BadRequest(BaseApiResponse<UploadAvatarResponse>.ErrorResult("File is required", new[] { "Missing file." }));

                var ct = file.ContentType ?? string.Empty;
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(ct))
                    return BadRequest(BaseApiResponse<UploadAvatarResponse>.ErrorResult("Unsupported image ContentType", new[] { $"ContentType: {ct}" }));

                var ext = ct.ToLowerInvariant() switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    _ => ""
                };

                var now = VietnamTime.Now;
                var objectName = $"users/{userId:D}/avatar/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";
                await using var stream = file.OpenReadStream();
                await _storage.UploadObjectAsync(objectName, stream, ct, HttpContext.RequestAborted);

                var bucketId = _configuration["Appwrite:BucketId"] ?? "";
                var media = new MediaFile
                {
                    OwnerUserId = userId,
                    Bucket = bucketId,
                    ObjectName = objectName,
                    OriginalFileName = file.FileName,
                    ContentType = ct,
                    SizeBytes = file.Length,
                    MediaType = "image",
                    IsPublic = true
                };
                var created = await _mediaFiles.CreateAsync(media);

                user.AvatarMediaFileId = created.Id;
                user.AvatarUrl = objectName; // store objectName; GetProfile can resolve to URL later if needed
                user.UpdatedAt = VietnamTime.Now;
                await _db.SaveChangesAsync();

                // return public view URL (no token) – relies on bucket public-read.
                var endpoint = (_configuration["Appwrite:Endpoint"] ?? "https://syd.cloud.appwrite.io/v1").TrimEnd('/');
                var projectId = _configuration["Appwrite:ProjectId"] ?? "";
                var fileId = ToFileId(objectName);
                var url = $"{endpoint}/storage/buckets/{bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(projectId)}";

                return Ok(BaseApiResponse<UploadAvatarResponse>.SuccessResult(new UploadAvatarResponse
                {
                    MediaFileId = created.Id,
                    ObjectName = objectName,
                    Url = url
                }, "Avatar uploaded successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<UploadAvatarResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    BaseApiResponse<UploadAvatarResponse>.ErrorResult("An error occurred while uploading avatar", new[] { ex.Message }));
            }
        }

        private static string ToFileId(string objectName)
        {
            var normalized = objectName.Trim();
            var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(normalized));
            var hex = Convert.ToHexString(hash).ToLowerInvariant();
            return $"f_{hex[..34]}";
        }
    }
}

public sealed class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}

public sealed class UploadAvatarResponse
{
    public int MediaFileId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}