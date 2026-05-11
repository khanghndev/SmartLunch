using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.CompanyContracts.SignOrganizationContract;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetMyOrganizationContracts;
using SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetOrganizationContract;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Hợp đồng theo tổ chức: xem chi tiết và ký số sau checkout (không dùng master-data / Admin).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/company/contracts")]
[Authorize(Policy = "roles:Company,Organization,Khách hàng doanh nghiệp")]
public class CompanyContractController : ControllerBase
{
    private readonly ILogger<CompanyContractController> _logger;
    private readonly IMediator _mediator;

    public CompanyContractController(ILogger<CompanyContractController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>Danh sách hợp đồng gắn các tổ chức mà user đang thuộc (active membership).</summary>
    [HttpGet]
    public async Task<ActionResult<BaseApiResponse<GetMyOrganizationContractsResponse>>> GetMyContracts()
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetMyOrganizationContractsQuery(userId));
            return Ok(BaseApiResponse<GetMyOrganizationContractsResponse>.SuccessResult(
                response,
                "Contracts retrieved successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            if (IsInvalidUserContext(ex))
                return Unauthorized(BaseApiResponse<GetMyOrganizationContractsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<GetMyOrganizationContractsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Company list contracts");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMyOrganizationContractsResponse>.ErrorResult(
                    "An error occurred while listing contracts",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Lấy hợp đồng nếu user thuộc đúng tổ chức của hợp đồng (để xem lại trước / sau ký).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseApiResponse<GetContractResponse>>> GetContract(int id)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetOrganizationContractQuery(id, userId));
            return Ok(BaseApiResponse<GetContractResponse>.SuccessResult(response, "Contract retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetContractResponse>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            if (IsInvalidUserContext(ex))
                return Unauthorized(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Company get contract {ContractId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractResponse>.ErrorResult("An error occurred while retrieving the contract", new[] { ex.Message }));
        }
    }

    /// <summary>Ký số hợp đồng (payload giống master-data POST .../sign).</summary>
    [HttpPost("{id:int}/sign")]
    public async Task<ActionResult<BaseApiResponse<GetContractResponse>>> SignContract(int id, [FromBody] SignContractRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new SignOrganizationContractCommand(id, userId, request));
            return Ok(BaseApiResponse<GetContractResponse>.SuccessResult(response, "Contract signed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetContractResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            if (IsInvalidUserContext(ex))
                return Unauthorized(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Company sign contract {ContractId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractResponse>.ErrorResult("An error occurred while signing the contract", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }

    private static bool IsInvalidUserContext(UnauthorizedAccessException ex) =>
        string.Equals(ex.Message, "Invalid user context.", StringComparison.Ordinal);
}
