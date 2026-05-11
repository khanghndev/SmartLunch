using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateActualIntakeFromProposal;
using SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateIngredientIntakeProposal;
using SmartLunch.Backend.Service.Application.Commands.IngredientIntake.ReviewIngredientIntakeProposal;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposal;
using SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposals;
using SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIntakeProposalReviewHistory;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Phiếu đề xuất nhập nguyên liệu — nhân viên kho (Staff / Nhân viên); Admin xem tất cả.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ingredient-intake-proposals")]
[Authorize(Policy = "roles:Staff,Admin,WarehouseStaff,Manager")]
public class IngredientIntakeProposalController : ControllerBase
{
    private readonly ILogger<IngredientIntakeProposalController> _logger;
    private readonly IMediator _mediator;

    public IngredientIntakeProposalController(
        ILogger<IngredientIntakeProposalController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Danh sách phiếu: nhân viên chỉ thấy phiếu của mình; Admin thấy tất cả.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:ingredient_intake_proposals.read")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientIntakeProposalsResponse>>> GetProposals(
        [FromQuery] GetIngredientIntakeProposalsRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetIngredientIntakeProposalsQuery(request, userId));
            return Ok(BaseApiResponse<GetIngredientIntakeProposalsResponse>.SuccessResult(
                response,
                "Intake proposals retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetIngredientIntakeProposalsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetIngredientIntakeProposalsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing intake proposals");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientIntakeProposalsResponse>.ErrorResult(
                    "An error occurred while listing intake proposals",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Lịch sử duyệt phiếu của quản lý: chỉ phiếu đã xử lý (không còn submitted).
    /// Nhân viên kho chỉ thấy phiếu do mình tạo; Admin thấy toàn hệ thống.
    /// </summary>
    [HttpGet("review-history")]
    [Authorize(Policy = "permission:ingredient_intake_proposals.read")]
    public async Task<ActionResult<BaseApiResponse<GetIntakeProposalReviewHistoryResponse>>> GetReviewHistory(
        [FromQuery] GetIntakeProposalReviewHistoryRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetIntakeProposalReviewHistoryQuery(request, userId));
            return Ok(BaseApiResponse<GetIntakeProposalReviewHistoryResponse>.SuccessResult(
                response,
                "Intake proposal review history retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetIntakeProposalReviewHistoryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetIntakeProposalReviewHistoryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading intake proposal review history");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIntakeProposalReviewHistoryResponse>.ErrorResult(
                    "An error occurred while loading review history",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Quản lý duyệt / từ chối phiếu đề xuất (trạng thái submitted).
    /// </summary>
    [HttpPost("{proposalId:int}/review")]
    [Authorize(Policy = "roles:Admin,Manager")]
    [Authorize(Policy = "permission:ingredient_intake_proposals.update")]
    public async Task<ActionResult<BaseApiResponse<ReviewIngredientIntakeProposalResponse>>> ReviewProposal(
        int proposalId,
        [FromBody] ReviewIngredientIntakeProposalRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new ReviewIngredientIntakeProposalCommand(proposalId, request, userId));
            return Ok(BaseApiResponse<ReviewIngredientIntakeProposalResponse>.SuccessResult(
                response,
                "Proposal reviewed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<ReviewIngredientIntakeProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ReviewIngredientIntakeProposalResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<ReviewIngredientIntakeProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing intake proposal {ProposalId}", proposalId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ReviewIngredientIntakeProposalResponse>.ErrorResult(
                    "An error occurred while reviewing the proposal",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Phiếu nhập kho thực tế từ phiếu đã duyệt: xác nhận đạt chuẩn, cộng tồn kho, đánh dấu fulfilled.
    /// </summary>
    [HttpPost("{proposalId:int}/actual-receipt")]
    [Authorize(Policy = "permission:ingredient_actual_intakes.create")]
    public async Task<ActionResult<BaseApiResponse<CreateActualIntakeFromProposalResponse>>> CreateActualReceipt(
        int proposalId,
        [FromBody] CreateActualIntakeFromProposalRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CreateActualIntakeFromProposalCommand(proposalId, request, userId));
            return Ok(BaseApiResponse<CreateActualIntakeFromProposalResponse>.SuccessResult(
                response,
                "Actual intake recorded and inventory updated successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateActualIntakeFromProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<CreateActualIntakeFromProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateActualIntakeFromProposalResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateActualIntakeFromProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating actual intake for proposal {ProposalId}", proposalId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateActualIntakeFromProposalResponse>.ErrorResult(
                    "An error occurred while recording actual intake",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Chi tiết một phiếu (nhân viên chỉ xem được phiếu của chính mình).
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "permission:ingredient_intake_proposals.read")]
    public async Task<ActionResult<BaseApiResponse<IngredientIntakeProposalDetailDto>>> GetProposal(int id)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new GetIngredientIntakeProposalQuery(id, userId));
            if (dto == null)
            {
                return NotFound(BaseApiResponse<IngredientIntakeProposalDetailDto>.NotFoundResult(
                    $"Intake proposal {id} was not found."));
            }

            return Ok(BaseApiResponse<IngredientIntakeProposalDetailDto>.SuccessResult(
                dto,
                "Intake proposal retrieved successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<IngredientIntakeProposalDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading intake proposal {ProposalId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<IngredientIntakeProposalDetailDto>.ErrorResult(
                    "An error occurred while loading the intake proposal",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Tạo phiếu đề xuất nhập nguyên liệu (trạng thái submitted).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permission:ingredient_intake_proposals.create")]
    public async Task<ActionResult<BaseApiResponse<CreateIngredientIntakeProposalResponse>>> CreateProposal(
        [FromBody] CreateIngredientIntakeProposalRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CreateIngredientIntakeProposalCommand(request, userId));
            return Ok(BaseApiResponse<CreateIngredientIntakeProposalResponse>.SuccessResult(
                response,
                "Intake proposal created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateIngredientIntakeProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<CreateIngredientIntakeProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateIngredientIntakeProposalResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateIngredientIntakeProposalResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating intake proposal");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateIngredientIntakeProposalResponse>.ErrorResult(
                    "An error occurred while creating the intake proposal",
                    new[] { ex.Message }));
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
