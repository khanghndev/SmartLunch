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
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPayOSClient _payOSClient;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<InitiateOrganizationMealPaymentCommandHandler> _logger;

    public InitiateOrganizationMealPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IPayOSClient payOSClient,
        IUserRepository userRepository,
        ILogger<InitiateOrganizationMealPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
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

        await EnsureOrderAwaitingPaymentAsync(order, cancellationToken);

        var pendingPayment = order.Payments.FirstOrDefault(p =>
            string.Equals(p.Method, "payos", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase));

        if (pendingPayment == null)
            throw new InvalidOperationException("Không tìm thấy khoản đặt cọc chờ thanh toán cho đơn này.");

        var depositVnd = (int)Math.Round(pendingPayment.Amount, MidpointRounding.AwayFromZero);
        if (depositVnd < 1)
            throw new ArgumentException("Deposit amount is invalid.");

        var payer = await _userRepository.GetByIdAsync(command.UserId);
        var payInput = BuildPayOsInput(req, order, pendingPayment, depositVnd, payer);

        var payOs = await CreateOrReusePayOsSessionAsync(
            payInput,
            pendingPayment,
            order,
            depositVnd,
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
            AlreadyPaidSynced = false,
        };
    }

    private async Task EnsureOrderAwaitingPaymentAsync(Order order, CancellationToken cancellationToken)
    {
        if (string.Equals(order.PaymentStatus, OrderPaymentStatus.AwaitingPayment, StringComparison.OrdinalIgnoreCase))
            return;

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
            return;
        }

        throw new InvalidOperationException(
            "Đơn hàng chưa ở trạng thái chờ thanh toán. Vui lòng hoàn tất ký phụ lục.");
    }

    private static PayOSCreatePaymentInput BuildPayOsInput(
        DTOs.Request.OrganizationMealOrders.InitiateOrganizationMealPaymentRequest req,
        Order order,
        Payment pendingPayment,
        int depositVnd,
        User? payer)
    {
        var buyerName = payer == null
            ? null
            : string.Join(
                " ",
                new[] { payer.FirstName, payer.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        if (string.IsNullOrEmpty(buyerName))
            buyerName = payer?.Username;

        return new PayOSCreatePaymentInput
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
    }

    private async Task<PayOSCreatePaymentResult> CreateOrReusePayOsSessionAsync(
        PayOSCreatePaymentInput payInput,
        Payment pendingPayment,
        Order order,
        int depositVnd,
        CancellationToken cancellationToken)
    {
        var created = await _payOSClient.CreatePaymentRequestAsync(payInput, cancellationToken);
        if (created.Success && !string.IsNullOrWhiteSpace(created.CheckoutUrl))
            return created;

        if (!IsDuplicatePayOsOrderError(created))
            return created;

        _logger.LogInformation(
            "PayOS orderCode {OrderCode} exists for order {OrderId}; reusing or creating fresh session.",
            pendingPayment.Id,
            order.Id);

        var existing = await _payOSClient.GetPaymentRequestAsync(pendingPayment.Id, cancellationToken);
        if (existing.Success)
        {
            if (IsPayOsPaidStatus(existing.Status))
            {
                if (IsPayOsAmountMatched(existing.Amount, depositVnd))
                {
                    await MarkPayOsDepositPaidLocallyAsync(pendingPayment, order, cancellationToken);
                    return new PayOSCreatePaymentResult
                    {
                        Success = false,
                        Message =
                            "PayOS ghi nhận giao dịch đã thanh toán. Vui lòng tải lại trang đơn hàng để xem trạng thái mới.",
                    };
                }

                _logger.LogWarning(
                    "PayOS reports PAID for payment {PaymentId} but amount mismatch; creating fresh session.",
                    pendingPayment.Id);
            }
            else if (!string.IsNullOrWhiteSpace(existing.CheckoutUrl))
            {
                return new PayOSCreatePaymentResult
                {
                    Success = true,
                    Status = existing.Status,
                    CheckoutUrl = existing.CheckoutUrl,
                    QrCode = existing.QrCode,
                    Amount = existing.Amount ?? depositVnd,
                    Message = "Đang chuyển tới trang thanh toán PayOS (phiên đã tạo trước đó).",
                };
            }
        }

        return await CreateFreshPayOsSessionAsync(payInput, pendingPayment, order, depositVnd, cancellationToken);
    }

    private async Task<PayOSCreatePaymentResult> CreateFreshPayOsSessionAsync(
        PayOSCreatePaymentInput payInput,
        Payment stalePending,
        Order order,
        int depositVnd,
        CancellationToken cancellationToken)
    {
        await _payOSClient.CancelPaymentRequestAsync(
            stalePending.Id,
            "Tạo phiên thanh toán mới",
            cancellationToken);

        stalePending.Status = "cancelled";
        await _orderRepository.CommitAsync();

        var freshPayment = new Payment
        {
            OrderId = order.Id,
            PayerId = stalePending.PayerId,
            PaymentDate = VietnamTime.Now,
            Amount = stalePending.Amount,
            Method = "payos",
            Status = "pending",
            CreatedAt = VietnamTime.Now,
        };

        freshPayment = await _paymentRepository.CreateForOrderAsync(freshPayment, cancellationToken);
        order.Payments.Add(freshPayment);

        payInput.OrderCode = freshPayment.Id;

        var created = await _payOSClient.CreatePaymentRequestAsync(payInput, cancellationToken);
        if (created.Success && !string.IsNullOrWhiteSpace(created.CheckoutUrl))
            return created;

        return new PayOSCreatePaymentResult
        {
            Success = false,
            Message = string.IsNullOrWhiteSpace(created.Message)
                ? "Không tạo được liên kết thanh toán PayOS. Vui lòng thử lại."
                : created.Message,
        };
    }

    private async Task MarkPayOsDepositPaidLocallyAsync(Payment pendingPayment, Order order, CancellationToken cancellationToken)
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

    private static bool IsPayOsAmountMatched(int? payOsAmount, int expectedVnd) =>
        payOsAmount.HasValue && payOsAmount.Value == expectedVnd;
}
