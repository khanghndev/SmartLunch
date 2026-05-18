using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.SignOrderAnnex;

public class SignOrderAnnexCommandHandler : IRequestHandler<SignOrderAnnexCommand, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderAnnexPdfService _orderAnnexPdfService;
    private readonly IContractRepository _contractRepository;

    public SignOrderAnnexCommandHandler(
        IOrderRepository orderRepository,
        IOrderAnnexPdfService orderAnnexPdfService,
        IContractRepository contractRepository)
    {
        _orderRepository = orderRepository;
        _orderAnnexPdfService = orderAnnexPdfService;
        _contractRepository = contractRepository;
    }

    public async Task<GetOrderResponse> Handle(SignOrderAnnexCommand request, CancellationToken cancellationToken)
    {
        var sig = (request.Request.DigitalSignature ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(sig))
            throw new ArgumentException("DigitalSignature is required.");

        var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} was not found.");

        if (order.UserId != request.UserId)
            throw new UnauthorizedAccessException("You may only sign annex for your own orders.");

        if (!string.IsNullOrWhiteSpace(order.AnnexPdfUrl))
        {
            var existing = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
            return new GetOrderResponse { Order = OrderDtoMapping.ToDto(existing ?? order) };
        }

        var buyer = ResolveBuyerDisplayName(order);
        var pdfUrl = await _orderAnnexPdfService.GenerateUploadAndResolveUrlAsync(
            order,
            buyer,
            sig,
            cancellationToken);

        order.AnnexPdfUrl = pdfUrl;
        order.AnnexSignedAt = VietnamTime.Now;
        order.UpdatedAt = VietnamTime.Now;

        if (string.Equals(order.PaymentStatus, OrderPaymentStatus.Unpaid, StringComparison.OrdinalIgnoreCase))
            order.PaymentStatus = OrderPaymentStatus.AwaitingPayment;

        var pendingDeposit = order.Payments.FirstOrDefault(p =>
            string.Equals(p.Method, "payos", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase));

        if (pendingDeposit != null && order.ContractId is int contractId && contractId > 0)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract != null)
            {
                contract.DepositAmount = pendingDeposit.Amount;
                contract.UpdatedAt = VietnamTime.Now;
                await _contractRepository.UpdateAsync(contract);
            }
        }

        await _orderRepository.CommitAsync();

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded ?? order) };
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
