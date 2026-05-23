using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Integration.PayOS;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

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

        if (!order.AnnexSignedAt.HasValue)
            throw new InvalidOperationException("Vui lòng ký phụ lục / hợp đồng đặt hàng trước khi thanh toán.");

        if (!string.Equals(order.PaymentStatus, OrderPaymentStatus.AwaitingPayment, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(order.PaymentStatus, OrderPaymentStatus.DepositPaid, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.PaymentStatus, OrderPaymentStatus.Partial, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.PaymentStatus, OrderPaymentStatus.Paid, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Đơn hàng đã được thanh toán đặt cọc hoặc đã thanh toán đủ.");
            }

            if (string.Equals(order.PaymentStatus, OrderPaymentStatus.Unpaid, StringComparison.OrdinalIgnoreCase))
            {
                order.PaymentStatus = OrderPaymentStatus.AwaitingPayment;
                order.UpdatedAt = VietnamTime.Now;
                await _orderRepository.CommitAsync();
            }
            else
            {
                throw new InvalidOperationException(
                    "Đơn hàng chưa ở trạng thái chờ thanh toán. Vui lòng hoàn tất ký phụ lục.");
            }
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

        var payInput = new PayOSCreatePaymentInput
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
        };

        var payOs = await _payOSClient.CreatePaymentRequestAsync(payInput, cancellationToken);

        if (!payOs.Success && IsDuplicatePayOsOrderError(payOs))
        {
            _logger.LogInformation(
                "PayOS orderCode {OrderCode} already exists; resolving existing payment link for order {OrderId}.",
                pendingPayment.Id,
                order.Id);

            payOs = await ResolveDuplicatePayOsOrderAsync(
                payInput,
                pendingPayment,
                order,
                depositVnd,
                cancellationToken);
        }

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
            AlreadyPaidSynced = payOs is { Success: true, AlreadyPaidSynced: true },
        };
    }

    private async Task<PayOSCreatePaymentResult> ResolveDuplicatePayOsOrderAsync(
        PayOSCreatePaymentInput payInput,
        Payment pendingPayment,
        Order order,
        int depositVnd,
        CancellationToken cancellationToken)
    {
        var existing = await _payOSClient.GetPaymentRequestAsync(pendingPayment.Id, cancellationToken);
        if (existing.Success)
        {
            if (IsPayOsPaidStatus(existing.Status))
            {
                await MarkPayOsDepositPaidLocallyAsync(pendingPayment, order, cancellationToken);
                return new PayOSCreatePaymentResult
                {
                    Success = true,
                    AlreadyPaidSynced = true,
                    Message = "Khoản đặt cọc đã được thanh toán. Trạng thái đơn đã được cập nhật.",
                };
            }

            if (IsPayOsCheckoutOpenStatus(existing.Status) && !string.IsNullOrWhiteSpace(existing.CheckoutUrl))
            {
                return new PayOSCreatePaymentResult
                {
                    Success = true,
                    Status = existing.Status,
                    CheckoutUrl = existing.CheckoutUrl,
                    QrCode = existing.QrCode,
                    Amount = existing.Amount ?? depositVnd,
                    Message = "Đang chuyển tới phiên thanh toán đã tạo trước đó.",
                };
            }
        }

        await _payOSClient.CancelPaymentRequestAsync(
            pendingPayment.Id,
            "Khách yêu cầu thanh toán lại",
            cancellationToken);

        var retry = await _payOSClient.CreatePaymentRequestAsync(payInput, cancellationToken);
        if (!retry.Success)
        {
            retry = new PayOSCreatePaymentResult
            {
                Success = false,
                Message = retry.Message.Contains("tồn tại", StringComparison.OrdinalIgnoreCase)
                    ? "Phiên thanh toán trước vẫn còn trên PayOS. Vui lòng thử lại sau vài phút hoặc liên hệ hỗ trợ."
                    : retry.Message,
            };
        }

        return retry;
    }

    private async Task MarkPayOsDepositPaidLocallyAsync(
        Payment pendingPayment,
        Order order,
        CancellationToken cancellationToken)
    {
        if (string.Equals(pendingPayment.Status, "paid", StringComparison.OrdinalIgnoreCase))
            return;

        pendingPayment.Status = "paid";
        pendingPayment.PaymentDate = VietnamTime.Now;

        decimal paidSum = 0;
        foreach (var p in order.Payments)
        {
            var status = p.Id == pendingPayment.Id ? pendingPayment.Status : p.Status;
            if (string.Equals(status, "paid", StringComparison.OrdinalIgnoreCase))
                paidSum += p.Amount;
        }

        var derived = PaymentReconciliationDerivation.DerivePaymentStatus(order.TotalAmount, paidSum);
        if (string.Equals(derived, PaymentReconciliationDerivation.DerivedOverpaid, StringComparison.Ordinal))
            order.PaymentStatus = OrderPaymentStatus.Paid;
        else if (string.Equals(derived, OrderPaymentStatus.Partial, StringComparison.Ordinal) && paidSum > 0)
            order.PaymentStatus = OrderPaymentStatus.DepositPaid;
        else
            order.PaymentStatus = derived;

        order.UpdatedAt = VietnamTime.Now;
        await _orderRepository.CommitAsync();
        _logger.LogInformation(
            "Synced PayOS paid status locally for payment {PaymentId}, order {OrderId}.",
            pendingPayment.Id,
            order.Id);
    }

    private static bool IsDuplicatePayOsOrderError(PayOSCreatePaymentResult result)
    {
        var text = $"{result.Message} {result.Desc}".ToLowerInvariant();
        return text.Contains("tồn tại", StringComparison.Ordinal)
               || text.Contains("already exist", StringComparison.Ordinal)
               || text.Contains("duplicate", StringComparison.Ordinal);
    }

    private static bool IsPayOsPaidStatus(string? status) =>
        string.Equals(status, "PAID", StringComparison.OrdinalIgnoreCase);

    private static bool IsPayOsCheckoutOpenStatus(string? status) =>
        string.IsNullOrWhiteSpace(status)
        || string.Equals(status, "PENDING", StringComparison.OrdinalIgnoreCase)
        || string.Equals(status, "PROCESSING", StringComparison.OrdinalIgnoreCase);
}
