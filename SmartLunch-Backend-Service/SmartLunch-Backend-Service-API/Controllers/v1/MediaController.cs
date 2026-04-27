using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLunch.Backend.Service.Application.Commands.Media.ConfirmUpload;
using SmartLunch.Backend.Service.Application.Commands.Media.CreateUploadUrl;
using SmartLunch.Backend.Service.Application.Commands.Media.GetDownloadUrl;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Media;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using System.Net;
using System.Security.Claims;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Media upload/download controller (Appwrite Storage bucket)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly ILogger<MediaController> _logger;
    private readonly IMediator _mediator;
    private readonly IStorageService _storage;

    public MediaController(ILogger<MediaController> logger, IMediator mediator, IStorageService storage)
    {
        _logger = logger;
        _mediator = mediator;
        _storage = storage;
    }

    [HttpPost("upload-url")]
    public async Task<ActionResult<BaseApiResponse<CreateMediaUploadUrlResponse>>> CreateUploadUrl([FromBody] CreateMediaUploadUrlRequest request)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new CreateMediaUploadUrlCommand(userId, request));
            response.UploadUrl = BuildProxyUploadUrl(response.ObjectName);
            response.RequiredHeaders["Content-Type"] = request.ContentType;
            return Ok(BaseApiResponse<CreateMediaUploadUrlResponse>.SuccessResult(response, "Upload URL created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateMediaUploadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<CreateMediaUploadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating upload URL");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateMediaUploadUrlResponse>.ErrorResult("An error occurred while creating upload URL", new[] { ex.Message }));
        }
    }

    [HttpPost("confirm-upload")]
    public async Task<ActionResult<BaseApiResponse<ConfirmMediaUploadResponse>>> ConfirmUpload([FromBody] ConfirmMediaUploadRequest request)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new ConfirmMediaUploadCommand(userId, request));
            return Ok(BaseApiResponse<ConfirmMediaUploadResponse>.SuccessResult(response, "Upload confirmed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ConfirmMediaUploadResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ConfirmMediaUploadResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<ConfirmMediaUploadResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming upload");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ConfirmMediaUploadResponse>.ErrorResult("An error occurred while confirming upload", new[] { ex.Message }));
        }
    }

    [HttpGet("{id:int}/download-url")]
    public async Task<ActionResult<BaseApiResponse<GetMediaDownloadUrlResponse>>> GetDownloadUrl(int id, [FromQuery] int? expiresMinutes = null)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new GetMediaDownloadUrlCommand(userId, id, expiresMinutes));
            return Ok(BaseApiResponse<GetMediaDownloadUrlResponse>.SuccessResult(response, "Download URL created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetMediaDownloadUrlResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMediaDownloadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetMediaDownloadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating download URL");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMediaDownloadUrlResponse>.ErrorResult("An error occurred while creating download URL", new[] { ex.Message }));
        }
    }

    [HttpPut("upload-proxy")]
    [EnableRateLimiting("media-upload")]
    public async Task<ActionResult<BaseApiResponse<object>>> UploadProxy(
        [FromQuery] string objectName,
        [FromHeader(Name = "Content-Type")] string? contentType = null)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var expectedPrefix = $"users/{userId:D}/";
            if (string.IsNullOrWhiteSpace(objectName) || !objectName.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                return BadRequest(BaseApiResponse<object>.ErrorResult("Invalid objectName", new[] { "objectName must be under current user prefix." }));

            if (string.IsNullOrWhiteSpace(contentType))
                return BadRequest(BaseApiResponse<object>.ErrorResult("Content-Type is required", new[] { "Missing Content-Type header." }));

            if (Request.ContentLength is null or <= 0)
                return BadRequest(BaseApiResponse<object>.ErrorResult("Body is required", new[] { "Upload body is empty." }));

            await _storage.UploadObjectAsync(objectName, Request.Body, contentType, HttpContext.RequestAborted);
            return Ok(BaseApiResponse<object>.SuccessResult(new { ObjectName = objectName }, "Uploaded to Appwrite successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<object>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proxy uploading media");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while uploading media", new[] { ex.Message }));
        }
    }

    private int GetUserIdOrThrow()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user context");
        }

        return userId;
    }

    private string BuildProxyUploadUrl(string objectName)
    {
        var version = RouteData.Values.TryGetValue("version", out var raw) ? raw?.ToString() : "1";
        var encoded = Uri.EscapeDataString(objectName);
        return $"{Request.Scheme}://{Request.Host}/api/v{version}/Media/upload-proxy?objectName={encoded}";
    }
}

