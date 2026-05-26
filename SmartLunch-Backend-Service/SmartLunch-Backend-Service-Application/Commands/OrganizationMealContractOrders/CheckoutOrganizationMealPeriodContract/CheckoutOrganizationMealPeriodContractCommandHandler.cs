using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.CheckoutOrganizationMealPeriodContract;

public sealed class CheckoutOrganizationMealPeriodContractCommandHandler
    : IRequestHandler<CheckoutOrganizationMealPeriodContractCommand, CheckoutOrganizationMealPeriodContractResponse>
{
    private static readonly int[] AllowedDepositPercents = { 20, 25, 30, 35, 50 };

    private readonly IOrganizationMealPeriodContractDraftCache _draftCache;
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPromotionEngine _promotionEngine;
    private readonly IPromotionRepository _promotionRepository;
    private readonly IOrganizationOrderEmailService _orderEmailService;
    private readonly ILogger<CheckoutOrganizationMealPeriodContractCommandHandler> _logger;

    public CheckoutOrganizationMealPeriodContractCommandHandler(
        IOrganizationMealPeriodContractDraftCache draftCache,
        IUserOrganizationRepository userOrganizationRepository,
        IContractRepository contractRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IPromotionEngine promotionEngine,
        IPromotionRepository promotionRepository,
        IOrganizationOrderEmailService orderEmailService,
        ILogger<CheckoutOrganizationMealPeriodContractCommandHandler> logger)
    {
        _draftCache = draftCache;
        _userOrganizationRepository = userOrganizationRepository;
        _contractRepository = contractRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _promotionEngine = promotionEngine;
        _promotionRepository = promotionRepository;
        _orderEmailService = orderEmailService;
        _logger = logger;
    }

    public async Task<CheckoutOrganizationMealPeriodContractResponse> Handle(
        CheckoutOrganizationMealPeriodContractCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (string.IsNullOrWhiteSpace(req.DraftId))
            throw new ArgumentException("DraftId is required.");
        if (!AllowedDepositPercents.Contains(req.DepositPercent))
        {
            throw new ArgumentException(
                $"DepositPercent must be one of: {string.Join(", ", AllowedDepositPercents)}.");
        }

        var draft = await _draftCache.GetAsync(command.UserId, req.DraftId.Trim(), cancellationToken);
        if (draft == null || draft.UserId != command.UserId)
            throw new ArgumentException("Draft not found or expired. Call POST contract again.");

        if (draft.Delivery == null)
            throw new ArgumentException("Draft is missing delivery information.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            command.UserId,
            draft.OrganizationId);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this organization.");

        var recomputedSubtotal = OrganizationMealPeriodContractCalculator.ComputeTotalValue(
            draft.StartDate,
            draft.EndDate,
            draft.ExcludedDates,
            draft.MealsPerDay,
            draft.MealUnitPrice);
        var draftSubtotal = draft.SubtotalAmount ?? recomputedSubtotal;
        if (draftSubtotal != recomputedSubtotal)
            throw new ArgumentException("Draft subtotal is inconsistent. Call POST contract again.");

        var contract = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(draft.ContractId, cancellationToken)
            ?? throw new ArgumentException("Contract not found. Call POST contract again.");

        if (contract.SourceOrderId.HasValue)
            throw new InvalidOperationException("This contract has already been checked out.");

        var totalQuantity = draft.ServiceDays * draft.MealsPerDay;
        var evaluation = await _promotionEngine.EvaluateAsync(new OrderPromotionEvaluateInput
        {
            Channel = PromotionConstants.ChannelB2BOrg,
            UserId = command.UserId,
            OrganizationId = draft.OrganizationId,
            ContractId = contract.Id,
            ContractType = contract.ContractType,
            PromotionCode = draft.PromotionCode,
            PromotionId = draft.AppliedPromotionId,
            Subtotal = recomputedSubtotal,
            TotalQuantity = totalQuantity,
            Lines = new List<OrderPromotionLineInput>(),
        }, cancellationToken);

        if (evaluation.TotalAfter != draft.TotalAmount ||
            evaluation.DiscountAmount != draft.DiscountAmount ||
            evaluation.Subtotal != draftSubtotal)
        {
            throw new ArgumentException("Promotion on draft is no longer valid. Call POST contract again.");
        }

        var total = draft.TotalAmount;
        var scheduledUtc = VietnamTime.CalendarDateMidnight(draft.StartDate);

        var order = new Order
        {
            UserId = command.UserId,
            ContractId = contract.Id,
            OrderDate = VietnamTime.Now,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = VietnamTime.Now,
            InvoiceCode = await AllocateInvoiceCodeAsync(draft.StartDate, cancellationToken),
            SubtotalAmount = evaluation.Subtotal,
            DiscountAmount = evaluation.DiscountAmount,
            TotalAmount = total,
        };

        if (evaluation.Applied && evaluation.PromotionId is int promoId)
        {
            var promo = await _promotionRepository.GetByIdWithTargetsAsync(promoId, cancellationToken);
            if (promo != null)
                order.PromotionApplications.Add(_promotionEngine.BuildApplication(order, promo, evaluation));
        }

        OrganizationMealDeliveryValidator.ApplyToOrder(order, draft.Delivery);
        order.Deliveries.Add(new Delivery
        {
            DeliveryAddress = OrganizationMealDeliveryValidator.BuildFullAddress(draft.Delivery),
            DeliveryStatus = "pending",
            Notes = BuildDeliveryNotes(draft.Delivery),
            CreatedAt = VietnamTime.Now,
        });

        var depositDecimal = decimal.Round(total * (req.DepositPercent / 100m), 2, MidpointRounding.AwayFromZero);
        var depositVnd = (int)Math.Round(depositDecimal, MidpointRounding.AwayFromZero);
        if (depositVnd < 1)
            throw new ArgumentException("Deposit amount is too small.");

        order.Payments.Add(new Payment
        {
            PayerId = command.UserId,
            PaymentDate = VietnamTime.Now,
            Amount = depositDecimal,
            Method = "payos",
            Status = "pending",
            CreatedAt = VietnamTime.Now,
        });

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        contract.SourceOrderId = order.Id;
        contract.DepositAmount = depositDecimal;
        contract.TotalValue = total;
        contract.MealUnitPrice = draft.MealUnitPrice;
        contract.MealsPerDay = draft.MealsPerDay;
        contract.UpdatedAt = VietnamTime.Now;
        await _contractRepository.UpdateAsync(contract);

        await _draftCache.RemoveAsync(command.UserId, req.DraftId.Trim(), cancellationToken);

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id)
            ?? throw new InvalidOperationException("Order created but failed to reload.");

        try
        {
            await _orderEmailService.SendOrderConfirmationAsync(reloaded, req.DepositPercent, cancellationToken);
            reloaded.OrderConfirmationEmailSentAt = VietnamTime.Now;
            await _orderRepository.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Period contract order {OrderId} created but confirmation email failed.", reloaded.Id);
        }

        return new CheckoutOrganizationMealPeriodContractResponse
        {
            ContractId = contract.Id,
            Order = new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded) },
            DepositPercent = req.DepositPercent,
            DepositAmountVnd = depositVnd,
            PayOsMessage = "Hợp đồng theo kỳ đã tạo. Vui lòng ký phụ lục và thanh toán đặt cọc.",
        };
    }

    private static string? BuildDeliveryNotes(OrganizationMealOrderDraftDelivery delivery)
    {
        var parts = new List<string>
        {
            $"Người nhận: {delivery.RecipientName}",
            $"SĐT: {delivery.RecipientPhone}",
            $"Email: {delivery.RecipientEmail}",
            "[HĐ theo kỳ]",
        };
        if (!string.IsNullOrWhiteSpace(delivery.PreferredDeliveryTime))
            parts.Add($"Giờ giao: {delivery.PreferredDeliveryTime}");
        if (!string.IsNullOrWhiteSpace(delivery.DeliveryNotes))
            parts.Add(delivery.DeliveryNotes);
        var joined = string.Join(" | ", parts);
        return joined.Length <= 255 ? joined : joined[..255];
    }

    private async Task<string> AllocateInvoiceCodeAsync(DateOnly startDate, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 12; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var candidate = $"HK-{startDate:yyyyMMdd}-{suffix}";
            if (!await _orderRepository.InvoiceCodeExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        throw new InvalidOperationException("Could not allocate a unique invoice code.");
    }
}
