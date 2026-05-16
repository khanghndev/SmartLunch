using MediatR;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetOrderAnnexPreview;

public sealed record GetOrderAnnexPreviewQuery(int OrderId, int UserId) : IRequest<byte[]>;
