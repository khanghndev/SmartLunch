using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.ClearShoppingCart;

public record ClearShoppingCartCommand(int UserId) : IRequest<GetShoppingCartResponse>;
