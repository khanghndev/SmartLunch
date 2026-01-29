using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Media.ConfirmUpload;
using SmartLunch.Backend.Service.Application.Commands.Media.CreateUploadUrl;
using SmartLunch.Backend.Service.Application.Commands.Media.GetDownloadUrl;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Media;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;
using System.Net;
using System.Security.Claims;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Media upload/download controller (Firebase Storage signed URLs)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly ILogger<MediaController> _logger;
    private readonly IMediator _mediator;

    public MediaController(ILogger<MediaController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost("upload-url")]
    public async Task<ActionResult<BaseApiResponse<CreateMediaUploadUrlResponse>>> CreateUploadUrl([FromBody] CreateMediaUploadUrlRequest request)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new CreateMediaUploadUrlCommand(userId, request));
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

    [HttpGet("{id:guid}/download-url")]
    public async Task<ActionResult<BaseApiResponse<GetMediaDownloadUrlResponse>>> GetDownloadUrl(Guid id, [FromQuery] int? expiresMinutes = null)
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

    private Guid GetUserIdOrThrow()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user context");
        }

        return userId;
    }
}

