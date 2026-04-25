using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.AddCartLine;

public record AddCartLineCommand(int UserId, AddCartLineRequest Request) : IRequest<GetShoppingCartResponse>;
