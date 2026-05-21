using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemLogs;
using SmartLunch.Backend.Service.Application.Queries.Systems.BackupSystem;
using SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackups;
using SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackupFile;
using SmartLunch.Backend.Service.Application.Commands.Systems.DeleteSystemBackup;
using SmartLunch.Backend.Service.Application.Commands.Systems.RestoreSystem;
using SmartLunch.Backend.Service.Application.Commands.Systems.UpdateBackupSchedule;
using SmartLunch.Backend.Service.Application.Queries.Systems.GetBackupSchedule;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// System management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Super Admin,Admin")]
public class SystemController : ControllerBase
{
    private readonly ILogger<SystemController> _logger;
    private readonly IMediator _mediator;

    public SystemController(ILogger<SystemController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of Systems with pagination
    /// </summary>
    [HttpGet("log")]
    [Authorize(Policy = "permission:systems.log")]
    public async Task<ActionResult<BaseApiResponse<GetSystemLogsResponse>>> GetSystemLogs([FromQuery] GetSystemLogsRequest request)
    {
        try
        {
            var query = new GetSystemLogsQuery(
                request.Page,
                request.PageSize);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetSystemLogsResponse>.SuccessResult(response, "System logs retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetSystemLogsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving System logs");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetSystemLogsResponse>.ErrorResult("An error occurred while retrieving System logs", new[] { ex.Message }));
        }
    }

    /// <summary>Lấy cấu hình lịch sao lưu tự động.</summary>
    [HttpGet("backup/schedule")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<ActionResult<BaseApiResponse<BackupScheduleDto>>> GetBackupSchedule()
    {
        try
        {
            var response = await _mediator.Send(new GetBackupScheduleQuery());
            return Ok(BaseApiResponse<BackupScheduleDto>.SuccessResult(response, "Backup schedule retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving backup schedule");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<BackupScheduleDto>.ErrorResult("An error occurred while retrieving backup schedule", new[] { ex.Message }));
        }
    }

    /// <summary>Cập nhật lịch sao lưu tự động (ngày/giờ theo giờ VN).</summary>
    [HttpPut("backup/schedule")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<ActionResult<BaseApiResponse<BackupScheduleDto>>> UpdateBackupSchedule([FromBody] UpdateBackupScheduleRequest request)
    {
        try
        {
            var actorId = RequireUserId();
            var response = await _mediator.Send(new UpdateBackupScheduleCommand(request, actorId));
            return Ok(BaseApiResponse<BackupScheduleDto>.SuccessResult(response, "Backup schedule updated successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<BackupScheduleDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating backup schedule");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<BackupScheduleDto>.ErrorResult("An error occurred while updating backup schedule", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Backup
    /// </summary>
    [HttpPost("backup")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<ActionResult<BaseApiResponse<BackupSystemResponse>>> BackupSystem(int id)
    {
        try
        {
            var query = new BackupSystemQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<BackupSystemResponse>.SuccessResult(response, "System backup retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<BackupSystemResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving System backup with ID: {SystemId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<BackupSystemResponse>.ErrorResult("An error occurred while retrieving System backup", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// List backup files (.sql) metadata for Admin UI
    /// </summary>
    [HttpGet("backup")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<ActionResult<BaseApiResponse<GetSystemBackupsResponse>>> GetSystemBackups([FromQuery] GetSystemBackupsRequest request)
    {
        try
        {
            var query = new GetSystemBackupsQuery(request.Page, request.PageSize, request.IncludeDeleted, request.From, request.To);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetSystemBackupsResponse>.SuccessResult(response, "System backups retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetSystemBackupsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system backups");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetSystemBackupsResponse>.ErrorResult("An error occurred while retrieving system backups", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Download a backup file (.sql) by backup id
    /// </summary>
    [HttpGet("backup/{id:int}/download")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<IActionResult> DownloadBackup([FromRoute] int id)
    {
        var meta = await _mediator.Send(new GetSystemBackupFileQuery(id));
        return Redirect(meta.DownloadUrl);
    }

    /// <summary>Trả URL tải file (Appwrite) cho proxy MVC.</summary>
    [HttpGet("backup/{id:int}/url")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<ActionResult<BaseApiResponse<GetSystemBackupFileResponse>>> GetBackupDownloadUrl([FromRoute] int id)
    {
        try
        {
            var meta = await _mediator.Send(new GetSystemBackupFileQuery(id));
            return Ok(BaseApiResponse<GetSystemBackupFileResponse>.SuccessResult(meta, "Download URL retrieved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetSystemBackupFileResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backup download URL {BackupId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetSystemBackupFileResponse>.ErrorResult("An error occurred", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Soft delete a backup metadata record; optionally delete physical file
    /// </summary>
    [HttpDelete("backup/{id:int}")]
    [Authorize(Policy = "permission:systems.backup")]
    public async Task<ActionResult<BaseApiResponse<DeleteSystemBackupResponse>>> DeleteBackup([FromRoute] int id, [FromQuery] bool deleteFile = false)
    {
        try
        {
            var actorId = RequireUserId();
            var response = await _mediator.Send(new DeleteSystemBackupCommand(id, deleteFile, actorId));
            return Ok(BaseApiResponse<DeleteSystemBackupResponse>.SuccessResult(response, "Backup deleted successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<DeleteSystemBackupResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<DeleteSystemBackupResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<DeleteSystemBackupResponse>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<DeleteSystemBackupResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backup {BackupId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<DeleteSystemBackupResponse>.ErrorResult("An error occurred while deleting backup", new[] { ex.Message }));
        }
    }

    /// <summary>Tạo tài khoản mới; có thể gán <see cref="CreateSystemRequest.InitialRoleId"/> (một role).</summary>
    [HttpPost("restore")]
    [Authorize(Policy = "permission:systems.restore")]
    public async Task<ActionResult<BaseApiResponse<RestoreSystemResponse>>> RestoreSystem([FromBody] RestoreSystemRequest request)
    {
        try
        {
            var actorId = RequireUserId();
            var response = await _mediator.Send(new RestoreSystemCommand(request, actorId));
            return Ok(BaseApiResponse<RestoreSystemResponse>.SuccessResult(response, "System restored successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<RestoreSystemResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<RestoreSystemResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<RestoreSystemResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring System");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<RestoreSystemResponse>.ErrorResult("An error occurred while restoring System", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }
}
