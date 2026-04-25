using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.CheckoutCart;

public record CheckoutCartCommand(int UserId, CheckoutCartRequest Request) : IRequest<GetOrderResponse>;
