using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.CreateContract;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.DeleteContract;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.UpdateContract;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Queries.Contracts.GetContract;
using SmartLunch.Backend.Service.Application.Queries.Contracts.GetContracts;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Contract management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class ContractController : ControllerBase
{
    private readonly ILogger<ContractController> _logger;
    private readonly IMediator _mediator;

    public ContractController(ILogger<ContractController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of contracts with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:contracts.read")]
    public async Task<ActionResult<BaseApiResponse<GetContractsResponse>>> GetContracts([FromQuery] GetContractsRequest request)
    {
        try
        {
            var query = new GetContractsQuery(request.Page, request.PageSize, request.SearchTerm, request.PartnerId);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetContractsResponse>.SuccessResult(response, "Contracts retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contracts");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractsResponse>.ErrorResult("An error occurred while retrieving contracts", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get contract by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:contracts.read")]
    public async Task<ActionResult<BaseApiResponse<GetContractResponse>>> GetContract(Guid id)
    {
        try
        {
            var query = new GetContractQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetContractResponse>.SuccessResult(response, "Contract retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contract with ID: {ContractId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractResponse>.ErrorResult("An error occurred while retrieving contract", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Tạo hợp đồng cung cấp (số HĐ, thời hạn Start/End, mô tả khung cung cấp suất).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permission:contracts.update")]
    public async Task<ActionResult<BaseApiResponse<GetContractResponse>>> CreateContract([FromBody] CreateContractRequest request)
    {
        try
        {
            var response = await _mediator.Send(new CreateContractCommand(request));
            return Ok(BaseApiResponse<GetContractResponse>.SuccessResult(response, "Contract created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractResponse>.ErrorResult("An error occurred while creating contract", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Cập nhật hợp đồng.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:contracts.update")]
    public async Task<ActionResult<BaseApiResponse<GetContractResponse>>> UpdateContract(Guid id, [FromBody] UpdateContractRequest request)
    {
        try
        {
            var response = await _mediator.Send(new UpdateContractCommand(id, request));
            return Ok(BaseApiResponse<GetContractResponse>.SuccessResult(response, "Contract updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<GetContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract {ContractId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractResponse>.ErrorResult("An error occurred while updating contract", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Xóa hợp đồng nếu chưa có thanh toán đối tác; nếu đã có thì chuyển trạng thái cancelled.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "permission:contracts.update")]
    public async Task<ActionResult<BaseApiResponse<DeleteContractResponse>>> DeleteContract(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new DeleteContractCommand(id));
            return Ok(BaseApiResponse<DeleteContractResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<DeleteContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contract {ContractId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<DeleteContractResponse>.ErrorResult("An error occurred while deleting contract", new[] { ex.Message }));
        }
    }
}
