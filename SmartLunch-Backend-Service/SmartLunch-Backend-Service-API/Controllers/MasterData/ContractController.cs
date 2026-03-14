using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var query = new GetContractsQuery(request.Page, request.PageSize, request.SearchTerm);
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
}
