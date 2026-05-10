using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.CustomerTypes;
using SmartLunch.Backend.Service.Application.Queries.CustomerTypes.GetCustomerTypes;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// CustomerType controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/customer-types")]
[Authorize(Policy = "roles:Admin,Manager")]
public class CustomerTypeController : ControllerBase
{
    private readonly ILogger<CustomerTypeController> _logger;
    private readonly IMediator _mediator;

    public CustomerTypeController(ILogger<CustomerTypeController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:customer_types.read")]
    public async Task<ActionResult<BaseApiResponse<GetCustomerTypesResponse>>> GetAll()
    {
        try
        {
            var response = await _mediator.Send(new GetCustomerTypesQuery());
            return Ok(BaseApiResponse<GetCustomerTypesResponse>.SuccessResult(response, "Customer types retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer types");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetCustomerTypesResponse>.ErrorResult("An error occurred while retrieving customer types", new[] { ex.Message }));
        }
    }
}

