using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.MediaFiles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MediaFiles;
using SmartLunch.Backend.Service.Application.Queries.MediaFiles.GetMediaFile;
using SmartLunch.Backend.Service.Application.Queries.MediaFiles.GetMediaFiles;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// MediaFile management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class MediaFileController : ControllerBase
{
    private readonly ILogger<MediaFileController> _logger;
    private readonly IMediator _mediator;

    public MediaFileController(ILogger<MediaFileController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of mediafiles with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:mediafiles.read")]
    public async Task<ActionResult<BaseApiResponse<GetMediaFilesResponse>>> GetMediaFiles([FromQuery] GetMediaFilesRequest request)
    {
        try
        {
            var query = new GetMediaFilesQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetMediaFilesResponse>.SuccessResult(response, "MediaFiles retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMediaFilesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mediafiles");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMediaFilesResponse>.ErrorResult("An error occurred while retrieving mediafiles", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get mediafile by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:mediafiles.read")]
    public async Task<ActionResult<BaseApiResponse<GetMediaFileResponse>>> GetMediaFile(int id)
    {
        try
        {
            var query = new GetMediaFileQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetMediaFileResponse>.SuccessResult(response, "MediaFile retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMediaFileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mediafile with ID: {MediaFileId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMediaFileResponse>.ErrorResult("An error occurred while retrieving mediafile", new[] { ex.Message }));
        }
    }
}
