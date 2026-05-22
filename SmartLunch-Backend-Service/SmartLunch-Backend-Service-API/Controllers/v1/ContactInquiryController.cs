using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.ContactInquiries.CreateContactInquiry;
using SmartLunch.Backend.Service.Application.Commands.ContactInquiries.ReplyToContactInquiry;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;
using SmartLunch.Backend.Service.Application.Queries.ContactInquiries.GetManagerContactInquiries;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Liên hệ từ trang web — gửi công khai; quản lý xem và trả lời nội bộ.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contact-inquiries")]
public class ContactInquiryController : ControllerBase
{
    private readonly ILogger<ContactInquiryController> _logger;
    private readonly IMediator _mediator;

    public ContactInquiryController(ILogger<ContactInquiryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<BaseApiResponse<CreateContactInquiryResponse>>> Create(
        [FromBody] CreateContactInquiryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            int? userId = null;
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out var uid))
                userId = uid;

            var response = await _mediator.Send(new CreateContactInquiryCommand(request, userId), cancellationToken);
            return Ok(BaseApiResponse<CreateContactInquiryResponse>.SuccessResult(response, response.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateContactInquiryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact inquiry");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateContactInquiryResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpGet("manager")]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<GetManagerContactInquiriesResponse>>> GetForManager(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _mediator.Send(
                new GetManagerContactInquiriesQuery(page, pageSize, searchTerm, status), cancellationToken);
            return Ok(BaseApiResponse<GetManagerContactInquiriesResponse>.SuccessResult(response, "Contact inquiries retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading contact inquiries");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetManagerContactInquiriesResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost("{id:int}/reply")]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<ManagerContactInquiryListItemDto>>> Reply(
        int id,
        [FromBody] ReplyToContactInquiryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var managerId = RequireUserId();
            var dto = await _mediator.Send(
                new ReplyToContactInquiryCommand(id, managerId, request.Reply), cancellationToken);
            return Ok(BaseApiResponse<ManagerContactInquiryListItemDto>.SuccessResult(dto, "Reply saved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ManagerContactInquiryListItemDto>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ManagerContactInquiryListItemDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replying to contact inquiry {InquiryId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ManagerContactInquiryListItemDto>.ErrorResult("Failed", new[] { ex.Message }));
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
