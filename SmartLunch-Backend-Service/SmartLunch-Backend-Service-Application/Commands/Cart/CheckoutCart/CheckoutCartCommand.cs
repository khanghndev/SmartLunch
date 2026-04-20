using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.CheckoutCart;

public record CheckoutCartCommand(Guid UserId, CheckoutCartRequest Request) : IRequest<GetOrderResponse>;
