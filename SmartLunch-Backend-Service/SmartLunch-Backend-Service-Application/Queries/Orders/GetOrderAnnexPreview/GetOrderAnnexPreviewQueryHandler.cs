using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetOrderAnnexPreview;

public sealed class GetOrderAnnexPreviewQueryHandler : IRequestHandler<GetOrderAnnexPreviewQuery, byte[]>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderAnnexPdfService _orderAnnexPdfService;

    public GetOrderAnnexPreviewQueryHandler(
        IOrderRepository orderRepository,
        IOrderAnnexPdfService orderAnnexPdfService)
    {
        _orderRepository = orderRepository;
        _orderAnnexPdfService = orderAnnexPdfService;
    }

    public async Task<byte[]> Handle(GetOrderAnnexPreviewQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} was not found.");

        if (order.UserId != request.UserId)
            throw new UnauthorizedAccessException("You may only preview annex for your own orders.");

        var buyer = ResolveBuyerDisplayName(order);
        return _orderAnnexPdfService.GeneratePdfBytes(order, buyer, signatureDataUrl: null);
    }

    private static string ResolveBuyerDisplayName(Order order)
    {
        var fromContract = order.Contract?.Organization?.Name;
        if (!string.IsNullOrWhiteSpace(fromContract))
            return fromContract.Trim();

        var fromUser = order.User?.UserOrganizations?
            .Where(uo => uo.IsActive)
            .Select(uo => uo.Organization?.Name)
            .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));

        if (!string.IsNullOrWhiteSpace(fromUser))
            return fromUser.Trim();

        var name = string.Join(" ", new[] { order.User?.FirstName, order.User?.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        return string.IsNullOrEmpty(name) ? (order.User?.Username ?? "Khách hàng") : name;
    }
}
