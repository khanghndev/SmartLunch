using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.UpdateCartLine;

public record UpdateCartLineCommand(int UserId, int LineId, UpdateCartLineRequest Request) : IRequest<GetShoppingCartResponse>;
