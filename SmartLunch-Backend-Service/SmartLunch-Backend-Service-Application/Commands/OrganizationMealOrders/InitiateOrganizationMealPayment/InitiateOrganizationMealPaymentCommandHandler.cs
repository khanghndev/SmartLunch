using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Integration.PayOS;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.InitiateOrganizationMealPayment;

public sealed class InitiateOrganizationMealPaymentCommandHandler
    : IRequestHandler<InitiateOrganizationMealPaymentCommand, InitiateOrganizationMealPaymentResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPayOSClient _payOSClient;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<InitiateOrganizationMealPaymentCommandHandler> _logger;

    public InitiateOrganizationMealPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPayOSClient payOSClient,
        IUserRepository userRepository,
        ILogger<InitiateOrganizationMealPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _payOSClient = payOSClient;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<InitiateOrganizationMealPaymentResponse> Handle(
        InitiateOrganizationMealPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (req.OrderId <= 0)
            throw new ArgumentException("OrderId is required.");

        var order = await _orderRepository.GetByIdWithDetailsAsync(req.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order {req.OrderId} was not found.");

        if (order.UserId != command.UserId)
            throw new UnauthorizedAccessException("You do not have access to this order.");

        if (string.IsNullOrWhiteSpace(order.AnnexPdfUrl))
            throw new InvalidOperationException("Vui lòng ký phụ lục đặt hàng trước khi thanh toán.");

        if (!string.Equals(order.PaymentStatus, OrderPaymentStatus.AwaitingPayment, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(order.PaymentStatus, OrderPaymentStatus.DepositPaid, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.PaymentStatus, OrderPaymentStatus.Partial, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.PaymentStatus, OrderPaymentStatus.Paid, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Đơn hàng đã được thanh toán đặt cọc hoặc đã thanh toán đủ.");
            }

            throw new InvalidOperationException(
                "Đơn hàng chưa ở trạng thái chờ thanh toán. Vui lòng hoàn tất ký phụ lục.");
        }

        var pendingPayment = order.Payments.FirstOrDefault(p =>
            string.Equals(p.Method, "payos", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase));

        if (pendingPayment == null)
            throw new InvalidOperationException("Không tìm thấy khoản đặt cọc chờ thanh toán cho đơn này.");

        var depositVnd = (int)Math.Round(pendingPayment.Amount, MidpointRounding.AwayFromZero);
        if (depositVnd < 1)
            throw new ArgumentException("Deposit amount is invalid.");

        var payer = await _userRepository.GetByIdAsync(command.UserId);
        var buyerName = payer == null
            ? null
            : string.Join(
                " ",
                new[] { payer.FirstName, payer.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        if (string.IsNullOrEmpty(buyerName))
            buyerName = payer?.Username;

        var payOs = await _payOSClient.CreatePaymentRequestAsync(
            new PayOSCreatePaymentInput
            {
                OrderCode = pendingPayment.Id,
                Amount = depositVnd,
                Description = $"Đặt cọc đơn suất ăn #{order.Id}",
                ReturnUrl = req.ReturnUrl,
                CancelUrl = req.CancelUrl,
                BuyerName = buyerName,
                BuyerEmail = payer?.Email,
                BuyerPhone = payer?.PhoneNumber,
                Items = new[]
                {
                    new PayOSPaymentItemInput
                    {
                        Name = $"Đặt cọc hợp đồng / đơn #{order.Id}",
                        Quantity = 1,
                        Price = depositVnd,
                        Unit = "VND",
                    },
                },
            },
            cancellationToken);

        if (!payOs.Success)
            _logger.LogWarning("PayOS payment initiation failed for order {OrderId}: {Message}", order.Id, payOs.Message);

        return new InitiateOrganizationMealPaymentResponse
        {
            OrderId = order.Id,
            DepositAmountVnd = depositVnd,
            CheckoutUrl = payOs.CheckoutUrl,
            QrCode = payOs.QrCode,
            PayOsStatus = payOs.Status,
            PayOsMessage = payOs.Success ? null : payOs.Message,
        };
    }
}
