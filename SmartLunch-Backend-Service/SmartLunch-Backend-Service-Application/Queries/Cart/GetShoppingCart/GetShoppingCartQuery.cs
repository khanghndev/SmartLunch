using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Queries.Cart.GetShoppingCart;

public record GetShoppingCartQuery(Guid UserId) : IRequest<GetShoppingCartResponse>;
