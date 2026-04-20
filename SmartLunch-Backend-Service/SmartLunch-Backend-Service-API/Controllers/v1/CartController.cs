using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Cart.AddCartLine;
using SmartLunch.Backend.Service.Application.Commands.Cart.CheckoutCart;
using SmartLunch.Backend.Service.Application.Commands.Cart.ClearShoppingCart;
using SmartLunch.Backend.Service.Application.Commands.Cart.RemoveCartLine;
using SmartLunch.Backend.Service.Application.Commands.Cart.UpdateCartLine;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Queries.Cart.GetShoppingCart;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Giỏ hàng (Redis). TTL mặc định ~30 ngày — cấu hình <c>CartCache:TtlDays</c>.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly IMediator _mediator;

    public CartController(ILogger<CartController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<BaseApiResponse<GetShoppingCartResponse>>> GetCart()
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetShoppingCartQuery(userId));
            return Ok(BaseApiResponse<GetShoppingCartResponse>.SuccessResult(response, "Cart retrieved successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading cart");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShoppingCartResponse>.ErrorResult("An error occurred while reading the cart", new[] { ex.Message }));
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<BaseApiResponse<GetShoppingCartResponse>>> AddItem([FromBody] AddCartLineRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new AddCartLineCommand(userId, request));
            return Ok(BaseApiResponse<GetShoppingCartResponse>.SuccessResult(response, "Item added to cart"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetShoppingCartResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding cart item");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShoppingCartResponse>.ErrorResult("An error occurred while updating the cart", new[] { ex.Message }));
        }
    }

    [HttpPut("items/{lineId:guid}")]
    public async Task<ActionResult<BaseApiResponse<GetShoppingCartResponse>>> UpdateItem(Guid lineId, [FromBody] UpdateCartLineRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new UpdateCartLineCommand(userId, lineId, request));
            return Ok(BaseApiResponse<GetShoppingCartResponse>.SuccessResult(response, "Cart line updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetShoppingCartResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart line {LineId}", lineId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShoppingCartResponse>.ErrorResult("An error occurred while updating the cart", new[] { ex.Message }));
        }
    }

    [HttpDelete("items/{lineId:guid}")]
    public async Task<ActionResult<BaseApiResponse<GetShoppingCartResponse>>> RemoveItem(Guid lineId)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new RemoveCartLineCommand(userId, lineId));
            return Ok(BaseApiResponse<GetShoppingCartResponse>.SuccessResult(response, "Cart line removed"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetShoppingCartResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart line {LineId}", lineId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShoppingCartResponse>.ErrorResult("An error occurred while updating the cart", new[] { ex.Message }));
        }
    }

    [HttpDelete]
    public async Task<ActionResult<BaseApiResponse<GetShoppingCartResponse>>> ClearCart()
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new ClearShoppingCartCommand(userId));
            return Ok(BaseApiResponse<GetShoppingCartResponse>.SuccessResult(response, "Cart cleared"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetShoppingCartResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShoppingCartResponse>.ErrorResult("An error occurred while clearing the cart", new[] { ex.Message }));
        }
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> Checkout([FromBody] CheckoutCartRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CheckoutCartCommand(userId, request));
            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Checkout successful"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetOrderResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checkout cart to order");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult("An error occurred while checkout", new[] { ex.Message }));
        }
    }

    private Guid RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }
}
