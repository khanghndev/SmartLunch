using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Transactions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;
using SmartLunch.Backend.Service.Application.Queries.Transactions.GetTransaction;
using SmartLunch.Backend.Service.Application.Queries.Transactions.GetTransactions;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Transaction management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly IMediator _mediator;

    public TransactionController(ILogger<TransactionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of transactions with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:transactions.read")]
    public async Task<ActionResult<BaseApiResponse<GetTransactionsResponse>>> GetTransactions([FromQuery] GetTransactionsRequest request)
    {
        try
        {
            var query = new GetTransactionsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetTransactionsResponse>.SuccessResult(response, "Transactions retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetTransactionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetTransactionsResponse>.ErrorResult("An error occurred while retrieving transactions", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:transactions.read")]
    public async Task<ActionResult<BaseApiResponse<GetTransactionResponse>>> GetTransaction(int id)
    {
        try
        {
            var query = new GetTransactionQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetTransactionResponse>.SuccessResult(response, "Transaction retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetTransactionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction with ID: {TransactionId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetTransactionResponse>.ErrorResult("An error occurred while retrieving transaction", new[] { ex.Message }));
        }
    }
}
