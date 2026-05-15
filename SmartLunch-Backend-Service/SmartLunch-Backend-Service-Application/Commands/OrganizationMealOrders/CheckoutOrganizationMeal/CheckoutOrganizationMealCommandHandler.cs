using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Integration.PayOS;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
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
    private readonly IContractPdfService _contractPdfService;
    private readonly IPayOSClient _payOSClient;
    private readonly IUserRepository _userRepository;
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
        IContractPdfService contractPdfService,
        IPayOSClient payOSClient,
        IUserRepository userRepository,
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
        _contractPdfService = contractPdfService;
        _payOSClient = payOSClient;
        _userRepository = userRepository;
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

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            command.UserId,
            draft.OrganizationId);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this organization.");

        // var utcNow = DateTime.UtcNow;
        // foreach (var day in draft.Days)
        // {
        //     if (!OrganizationMealOrderDateWindow.IsDateInWindow(day.ServiceDate, utcNow))
        //     {
        //         throw new ArgumentException(
        //             "Draft is no longer valid for the current booking window. Prepare the contract again.");
        //     }
        // }

        var recomputedTotal = decimal.Round(
            draft.PricePerPortion * draft.TotalMainQuantity,
            2,
            MidpointRounding.AwayFromZero);
        if (recomputedTotal != draft.TotalAmount)
            throw new ArgumentException("Draft total is inconsistent. Call POST contract again.");

        var dishSlot = BuildDishSlotMap(draft);
        var mergedQty = MergeQuantities(draft);
        var dishIds = mergedQty.Keys.ToList();

        var dishes = await _dishRepository.GetByIdsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes are no longer available.");

        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not active.");
        }

        var total = recomputedTotal;

        var checkoutOrg = await _organizationRepository.GetByIdAsync(draft.OrganizationId);
        if (checkoutOrg == null || !checkoutOrg.IsActive)
            throw new ArgumentException("Organization is not available.");

        Contract? contract = await _contractRepository.GetActiveForOrganizationAsync(checkoutOrg.Id, cancellationToken);
        var wasNewContract = false;
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
                Description = $"Hợp đồng đặt suất đơn vị — đơn hàng {DateTime.UtcNow:yyyy-MM-dd}",
                SupplySchedule = null,
                StartDate = DateTime.UtcNow.Date,
                EndDate = null,
                TotalValue = total,
                MealUnitPrice = draft.PricePerPortion,
                DepositAmount = null,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
            };
            wasNewContract = true;
        }

        var scheduledDate = draft.MinServiceDate;
        var scheduledUtc = DateTime.SpecifyKind(scheduledDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var order = new Order
        {
            UserId = command.UserId,
            ContractId = contract.Id > 0 ? contract.Id : null,
            OrderDate = DateTime.UtcNow,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = DateTime.UtcNow,
            InvoiceCode = await AllocateOrganizationInvoiceCodeAsync(scheduledDate, cancellationToken),
        };

        foreach (var (dishId, quantity) in mergedQty)
        {
            var slot = dishSlot[dishId];
            var isMain = string.Equals(slot, "main", StringComparison.OrdinalIgnoreCase);
            var unitPrice = isMain ? draft.PricePerPortion : 0m;
            var lineTotal = decimal.Round(unitPrice * quantity, 2, MidpointRounding.AwayFromZero);
            order.OrderItems.Add(new OrderItem
            {
                DishId = dishId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = lineTotal,
            });
        }

        order.TotalAmount = total;

        var depositDecimal = decimal.Round(total * (req.DepositPercent / 100m), 2, MidpointRounding.AwayFromZero);
        var depositVnd = (int)Math.Round(depositDecimal, MidpointRounding.AwayFromZero);
        if (depositVnd < 1)
            throw new ArgumentException("Deposit amount is too small.");

        var payment = new Payment
        {
            PayerId = command.UserId,
            PaymentDate = DateTime.UtcNow,
            Amount = depositDecimal,
            Method = "payos",
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
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

        if (wasNewContract && contract != null)
        {
            var forPdf = await _contractRepository.GetByIdAsync(contract.Id)
                         ?? throw new InvalidOperationException("Contract created but reload failed before PDF.");

            if (forPdf.Partner != null)
            {
                var url = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
                    forPdf,
                    forPdf.Partner,
                    checkoutOrg,
                    cancellationToken);
                forPdf.ContractFileUrl = url;
                forPdf.UpdatedAt = DateTime.UtcNow;
                await _contractRepository.UpdateAsync(forPdf);
            }
        }

        if (!wasNewContract && contract != null && contract.Id > 0)
            await SyncExistingContractMealPricingAsync(contract.Id, draft, total, cancellationToken);

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
                OrderCode = payment.Id,
                Amount = depositVnd,
                Description = $"Đặt cọc {req.DepositPercent}% đơn suất ăn #{order.Id}",
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
            _logger.LogWarning("PayOS checkout failed for order {OrderId}: {Message}", order.Id, payOs.Message);

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id)
            ?? throw new InvalidOperationException("Order created but failed to reload.");

        return new CheckoutOrganizationMealResponse
        {
            Order = new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded) },
            DepositPercent = req.DepositPercent,
            DepositAmountVnd = depositVnd,
            CheckoutUrl = payOs.CheckoutUrl,
            QrCode = payOs.QrCode,
            PayOsStatus = payOs.Status,
            PayOsMessage = payOs.Success ? null : payOs.Message,
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

        persisted.UpdatedAt = DateTime.UtcNow;
        await _contractRepository.UpdateAsync(persisted);
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

    private static Dictionary<int, int> MergeQuantities(OrganizationMealOrderDraftPayload draft)
    {
        var qty = new Dictionary<int, int>();
        foreach (var day in draft.Days)
        {
            void Add(List<OrganizationMealOrderDraftLine> lines)
            {
                foreach (var line in lines)
                {
                    if (!qty.TryGetValue(line.DishId, out var s))
                        s = 0;
                    qty[line.DishId] = s + line.Quantity;
                }
            }

            Add(day.Main);
            Add(day.Side);
            Add(day.Soup);
        }

        return qty;
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
