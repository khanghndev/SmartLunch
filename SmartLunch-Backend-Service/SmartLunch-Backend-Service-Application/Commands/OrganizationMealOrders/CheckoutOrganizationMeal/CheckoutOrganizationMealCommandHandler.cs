using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.CheckoutOrganizationMeal;

public sealed class CheckoutOrganizationMealCommandHandler
    : IRequestHandler<CheckoutOrganizationMealCommand, CheckoutOrganizationMealResponse>
{
    private static readonly int[] AllowedDepositPercents = { 20, 25, 30, 35, 50 };

    private readonly IOrganizationMealOrderDraftCache _draftCache;
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPromotionEngine _promotionEngine;
    private readonly IPromotionRepository _promotionRepository;
    private readonly IOrganizationOrderEmailService _orderEmailService;
    private readonly ILogger<CheckoutOrganizationMealCommandHandler> _logger;

    public CheckoutOrganizationMealCommandHandler(
        IOrganizationMealOrderDraftCache draftCache,
        IUserOrganizationRepository userOrganizationRepository,
        IDishRepository dishRepository,
        IOrganizationRepository organizationRepository,
        IContractRepository contractRepository,
        IPartnerRepository partnerRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IPromotionEngine promotionEngine,
        IPromotionRepository promotionRepository,
        IOrganizationOrderEmailService orderEmailService,
        ILogger<CheckoutOrganizationMealCommandHandler> logger)
    {
        _draftCache = draftCache;
        _userOrganizationRepository = userOrganizationRepository;
        _dishRepository = dishRepository;
        _organizationRepository = organizationRepository;
        _contractRepository = contractRepository;
        _partnerRepository = partnerRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _promotionEngine = promotionEngine;
        _promotionRepository = promotionRepository;
        _orderEmailService = orderEmailService;
        _logger = logger;
    }

    public async Task<CheckoutOrganizationMealResponse> Handle(
        CheckoutOrganizationMealCommand command,
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

        var draftId = req.DraftId.Trim();
        var draft = await _draftCache.GetAsync(command.UserId, draftId, cancellationToken);
        if (draft == null || draft.UserId != command.UserId)
            throw new ArgumentException("Draft not found or expired. Call POST contract again.");

        if (draft.Delivery == null)
            throw new ArgumentException("Draft is missing delivery information. Call POST contract again.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            command.UserId,
            draft.OrganizationId);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this organization.");

        // var utcNow = VietnamTime.Now;
        // foreach (var day in draft.Days)
        // {
        //     if (!OrganizationMealOrderDateWindow.IsDateInWindow(day.ServiceDate, utcNow))
        //     {
        //         throw new ArgumentException(
        //             "Draft is no longer valid for the current booking window. Prepare the contract again.");
        //     }
        // }

        var recomputedSubtotal = decimal.Round(
            draft.PricePerPortion * draft.TotalMainQuantity,
            2,
            MidpointRounding.AwayFromZero);

        var draftSubtotal = draft.SubtotalAmount ?? recomputedSubtotal;
        if (draftSubtotal != recomputedSubtotal)
            throw new ArgumentException("Draft subtotal is inconsistent. Call POST contract again.");

        OrganizationMealSlotBalance.ValidateDraftDays(draft.Days);

        var checkoutOrg = await _organizationRepository.GetByIdAsync(draft.OrganizationId);
        if (checkoutOrg == null || !checkoutOrg.IsActive)
            throw new ArgumentException("Organization is not available.");

        Contract? existingContract = null;
        if (draft.ContractId > 0)
            existingContract = await _contractRepository.GetByIdAsync(draft.ContractId);

        if (existingContract != null && (existingContract.IsDigitallySigned || existingContract.SourceOrderId.HasValue))
            existingContract = null;

        var promoLines = BuildPromotionLines(draft);
        var evaluation = await _promotionEngine.EvaluateAsync(new OrderPromotionEvaluateInput
        {
            Channel = PromotionConstants.ChannelB2BOrg,
            UserId = command.UserId,
            OrganizationId = draft.OrganizationId,
            ContractId = existingContract?.Id,
            ContractType = existingContract?.ContractType,
            PromotionCode = draft.PromotionCode,
            Subtotal = recomputedSubtotal,
            TotalQuantity = draft.TotalMainQuantity,
            Lines = promoLines,
        }, cancellationToken);

        if (evaluation.TotalAfter != draft.TotalAmount ||
            evaluation.DiscountAmount != draft.DiscountAmount ||
            evaluation.Subtotal != draftSubtotal)
        {
            throw new ArgumentException("Promotion on draft is no longer valid. Call POST contract again.");
        }

        var dishSlot = BuildDishSlotMap(draft);
        var dishIds = dishSlot.Keys.ToList();

        var dishes = await _dishRepository.GetByIdsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes are no longer available.");

        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not active.");
        }

        var total = draft.TotalAmount;

        Contract? contract = existingContract;
        if (contract == null)
        {
            var (partners, _) = await _partnerRepository.GetPartnersAsync(
                page: 1,
                pageSize: 1,
                searchTerm: null,
                isActive: true);

            var partner = partners.FirstOrDefault()
                ?? throw new InvalidOperationException("No active partner found to create contract.");

            contract = new Contract
            {
                PartnerId = partner.Id,
                OrganizationId = checkoutOrg.Id,
                ContractType = "Order-Based",
                Description = $"Hợp đồng đặt suất đơn vị — đơn hàng {VietnamTime.Now:yyyy-MM-dd}",
                SupplySchedule = null,
                StartDate = VietnamTime.Now.Date,
                EndDate = null,
                TotalValue = total,
                MealUnitPrice = draft.PricePerPortion,
                DepositAmount = null,
                Status = "active",
                CreatedAt = VietnamTime.Now,
            };
        }

        var scheduledDate = draft.MinServiceDate;
        var scheduledUtc = VietnamTime.CalendarDateMidnight(scheduledDate);

        var order = new Order
        {
            UserId = command.UserId,
            ContractId = contract.Id > 0 ? contract.Id : null,
            OrderDate = VietnamTime.Now,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = VietnamTime.Now,
            InvoiceCode = await AllocateOrganizationInvoiceCodeAsync(scheduledDate, cancellationToken),
        };

        OrganizationMealDeliveryValidator.ApplyToOrder(order, draft.Delivery);

        var deliveryAddressFull = OrganizationMealDeliveryValidator.BuildFullAddress(draft.Delivery);
        order.Deliveries.Add(new Delivery
        {
            DeliveryAddress = deliveryAddressFull,
            DeliveryStatus = "pending",
            Notes = BuildDeliveryNotes(draft.Delivery),
            CreatedAt = VietnamTime.Now,
        });

        foreach (var day in draft.Days.OrderBy(d => d.ServiceDate))
        {
            void AddDayLines(List<OrganizationMealOrderDraftLine> lines, string slot)
            {
                var isMain = string.Equals(slot, "main", StringComparison.OrdinalIgnoreCase);
                foreach (var line in lines)
                {
                    var unitPrice = isMain ? draft.PricePerPortion : 0m;
                    var lineTotal = decimal.Round(unitPrice * line.Quantity, 2, MidpointRounding.AwayFromZero);
                    order.OrderItems.Add(new OrderItem
                    {
                        DishId = line.DishId,
                        Quantity = line.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = lineTotal,
                        ServiceDate = day.ServiceDate,
                    });
                }
            }

            AddDayLines(day.Main, "main");
            AddDayLines(day.Side, "side");
            AddDayLines(day.Soup, "soup");
        }

        order.SubtotalAmount = evaluation.Subtotal;
        order.DiscountAmount = evaluation.DiscountAmount;
        order.TotalAmount = total;

        if (evaluation.Applied && evaluation.PromotionId is int promoId)
        {
            var promo = await _promotionRepository.GetByIdWithTargetsAsync(promoId, cancellationToken);
            if (promo != null)
                order.PromotionApplications.Add(_promotionEngine.BuildApplication(order, promo, evaluation));
        }

        var depositDecimal = decimal.Round(total * (req.DepositPercent / 100m), 2, MidpointRounding.AwayFromZero);
        var depositVnd = (int)Math.Round(depositDecimal, MidpointRounding.AwayFromZero);
        if (depositVnd < 1)
            throw new ArgumentException("Deposit amount is too small.");

        var payment = new Payment
        {
            PayerId = command.UserId,
            PaymentDate = VietnamTime.Now,
            Amount = depositDecimal,
            Method = "payos",
            Status = "pending",
            CreatedAt = VietnamTime.Now,
        };
        order.Payments.Add(payment);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (contract != null && contract.Id == 0)
                contract = await _contractRepository.CreateAsync(contract);

            order.ContractId = contract?.Id;
            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        await _draftCache.RemoveAsync(command.UserId, draftId, cancellationToken);

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id)
            ?? throw new InvalidOperationException("Order created but failed to reload.");

        if (reloaded.ContractId is int cid && cid > 0)
        {
            var persistedContract = await _contractRepository.GetByIdAsync(cid)
                ?? throw new InvalidOperationException("Contract not found after checkout.");

            persistedContract.SourceOrderId = reloaded.Id;
            persistedContract.DepositAmount = depositDecimal;
            persistedContract.TotalValue = total;
            persistedContract.MealUnitPrice = draft.PricePerPortion;
            persistedContract.IsDigitallySigned = false;
            persistedContract.DigitalSignature = null;
            persistedContract.DigitallySignedAt = null;
            persistedContract.UpdatedAt = VietnamTime.Now;
            await _contractRepository.UpdateAsync(persistedContract);
        }
        else if (contract != null && contract.Id > 0)
        {
            await SyncExistingContractMealPricingAsync(contract.Id, draft, total, cancellationToken);
        }

        try
        {
            await _orderEmailService.SendOrderConfirmationAsync(reloaded, req.DepositPercent, cancellationToken);
            reloaded.OrderConfirmationEmailSentAt = VietnamTime.Now;
            await _orderRepository.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Order {OrderId} created but confirmation email failed.", reloaded.Id);
        }

        return new CheckoutOrganizationMealResponse
        {
            Order = new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded) },
            DepositPercent = req.DepositPercent,
            DepositAmountVnd = depositVnd,
            CheckoutUrl = null,
            QrCode = null,
            PayOsStatus = null,
            PayOsMessage = "Đơn đã tạo. Vui lòng ký phụ lục đặt hàng trước khi thanh toán đặt cọc.",
        };
    }

    private async Task SyncExistingContractMealPricingAsync(
        int contractId,
        OrganizationMealOrderDraftPayload draft,
        decimal orderTotal,
        CancellationToken cancellationToken)
    {
        var persisted = await _contractRepository.GetByIdAsync(contractId);
        if (persisted == null)
            return;

        persisted.MealUnitPrice = draft.PricePerPortion;
        if (string.Equals(persisted.ContractType, "Order-Based", StringComparison.OrdinalIgnoreCase))
            persisted.TotalValue = orderTotal;

        persisted.UpdatedAt = VietnamTime.Now;
        await _contractRepository.UpdateAsync(persisted);
    }

    private static List<OrderPromotionLineInput> BuildPromotionLines(OrganizationMealOrderDraftPayload draft)
    {
        var lines = new List<OrderPromotionLineInput>();
        foreach (var day in draft.Days)
        {
            foreach (var line in day.Main)
            {
                lines.Add(new OrderPromotionLineInput
                {
                    DishId = line.DishId,
                    Quantity = line.Quantity,
                    LineTotal = decimal.Round(
                        draft.PricePerPortion * line.Quantity,
                        2,
                        MidpointRounding.AwayFromZero),
                });
            }
        }

        return lines;
    }

    private static string? BuildDeliveryNotes(OrganizationMealOrderDraftDelivery delivery)
    {
        var parts = new List<string>
        {
            $"Người nhận: {delivery.RecipientName}",
            $"SĐT: {delivery.RecipientPhone}",
            $"Email: {delivery.RecipientEmail}",
        };
        if (!string.IsNullOrWhiteSpace(delivery.PreferredDeliveryTime))
            parts.Add($"Giờ giao: {delivery.PreferredDeliveryTime}");
        if (!string.IsNullOrWhiteSpace(delivery.DeliveryNotes))
            parts.Add(delivery.DeliveryNotes);
        var joined = string.Join(" | ", parts);
        return joined.Length <= 255 ? joined : joined[..255];
    }

    private static Dictionary<int, string> BuildDishSlotMap(OrganizationMealOrderDraftPayload draft)
    {
        var map = new Dictionary<int, string>();
        foreach (var day in draft.Days)
        {
            void Register(string slot, List<OrganizationMealOrderDraftLine> lines)
            {
                foreach (var line in lines)
                {
                    if (map.TryGetValue(line.DishId, out var existing) &&
                        !string.Equals(existing, slot, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException(
                            $"Draft has dish {line.DishId} in conflicting slots ('{existing}' vs '{slot}').");
                    }

                    map[line.DishId] = slot;
                }
            }

            Register("main", day.Main);
            Register("side", day.Side);
            Register("soup", day.Soup);
        }

        return map;
    }

    private async Task<string> AllocateOrganizationInvoiceCodeAsync(
        DateOnly scheduledDate,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 12; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var candidate = $"DV-{scheduledDate:yyyyMMdd}-{suffix}";
            if (!await _orderRepository.InvoiceCodeExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        throw new InvalidOperationException("Could not allocate a unique organization invoice code.");
    }
}
