using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Partners;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;
using SmartLunch.Backend.Service.Application.Queries.Partners.GetPartner;
using SmartLunch.Backend.Service.Application.Queries.Partners.GetPartners;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Partner (supplier) management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class PartnerController : ControllerBase
{
    private readonly ILogger<PartnerController> _logger;
    private readonly IMediator _mediator;

    public PartnerController(ILogger<PartnerController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of partners with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:partners.read")]
    public async Task<ActionResult<BaseApiResponse<GetPartnersResponse>>> GetPartners([FromQuery] GetPartnersRequest request)
    {
        try
        {
            var query = new GetPartnersQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetPartnersResponse>.SuccessResult(response, "Partners retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPartnersResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving partners");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPartnersResponse>.ErrorResult("An error occurred while retrieving partners", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get partner by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:partners.read")]
    public async Task<ActionResult<BaseApiResponse<GetPartnerResponse>>> GetPartner(Guid id)
    {
        try
        {
            var query = new GetPartnerQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetPartnerResponse>.SuccessResult(response, "Partner retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPartnerResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving partner with ID: {PartnerId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPartnerResponse>.ErrorResult("An error occurred while retrieving partner", new[] { ex.Message }));
        }
    }
}
