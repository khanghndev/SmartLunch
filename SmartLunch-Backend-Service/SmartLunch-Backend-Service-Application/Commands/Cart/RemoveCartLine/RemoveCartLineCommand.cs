using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.RemoveCartLine;

public record RemoveCartLineCommand(Guid UserId, Guid LineId) : IRequest<GetShoppingCartResponse>;
