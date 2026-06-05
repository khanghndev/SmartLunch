using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>
/// Suất ăn tuần theo HĐ Period-Based — lưu <c>contract_weekly_selections</c>,
/// tạo đơn fulfillment nội bộ (kitchen) gắn qua <see cref="ContractWeeklySelection.FulfillmentOrderId"/>.
/// </summary>
public sealed class OrganizationMealContractWeeklySelectionService
{
    private readonly IContractWeeklySelectionRepository _weeklyRepository;
    private readonly IOrderRepository _orderRepository;

    public OrganizationMealContractWeeklySelectionService(
        IContractWeeklySelectionRepository weeklyRepository,
        IOrderRepository orderRepository)
    {
        _weeklyRepository = weeklyRepository;
        _orderRepository = orderRepository;
    }

    public async Task EnsureWeeksSeededAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        if (!contract.EndDate.HasValue)
            return;

        var contractStart = DateOnly.FromDateTime(contract.StartDate);
        var contractEnd = DateOnly.FromDateTime(contract.EndDate.Value);
        var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToList();
        var existing = await _weeklyRepository.GetAllByContractIdAsync(contract.Id, cancellationToken: cancellationToken);
        var existingMondays = existing.Select(w => w.WeekMonday).ToHashSet();
        var added = false;

        foreach (var (monday, _) in OrganizationMealPeriodContractCalculator.EnumerateWeeks(contractStart, contractEnd))
        {
            if (existingMondays.Contains(monday))
                continue;

            var hasServiceDays = OrganizationMealPeriodContractCalculator
                .GetWeekServiceDates(monday, contractStart, contractEnd, excluded)
                .Any();
            if (!hasServiceDays)
                continue;

            await _weeklyRepository.AddAsync(new ContractWeeklySelection
            {
                ContractId = contract.Id,
                WeekMonday = monday,
                Status = ContractWeeklySelectionStatuses.Pending,
                CreatedAt = VietnamTime.Now,
            }, cancellationToken);
            added = true;
        }

        if (added)
            await _weeklyRepository.SaveChangesAsync(cancellationToken);

        await SyncLegacyWeekOrdersAsync(contract, cancellationToken);
    }

    /// <summary>Đồng bộ đơn fulfillment tuần cũ (Tuan-*) vào bảng weekly selections.</summary>
    public async Task SyncLegacyWeekOrdersAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        var selections = await _weeklyRepository.GetAllByContractIdAsync(
            contract.Id, includeItems: true, cancellationToken: cancellationToken);
        var changed = false;
        foreach (var sel in selections)
        {
            var needsHeaderSync = !ContractWeeklySelectionStatuses.IsFilled(sel.Status);
            var needsItemsSync = sel.Items.Count == 0;
            if (!needsHeaderSync && !needsItemsSync)
                continue;

            var order = await _orderRepository.GetContractWeekOrderAsync(
                contract.Id, sel.WeekMonday, cancellationToken);
            if (order == null || order.OrderItems.Count == 0)
                continue;

            if (needsHeaderSync)
            {
                sel.Status = ContractWeeklySelectionStatuses.Selected;
                sel.FulfillmentOrderId = order.Id;
                sel.SelectedAt = order.UpdatedAt ?? order.CreatedAt;
                changed = true;
            }
            else if (sel.FulfillmentOrderId != order.Id)
            {
                sel.FulfillmentOrderId = order.Id;
                changed = true;
            }

            if (needsItemsSync)
            {
                foreach (var item in order.OrderItems)
                {
                    if (!item.ServiceDate.HasValue)
                        continue;

                    sel.Items.Add(new ContractWeeklySelectionItem
                    {
                        ServiceDate = item.ServiceDate.Value,
                        DishId = item.DishId,
                        Quantity = item.Quantity,
                    });
                }

                changed = true;
            }

            sel.UpdatedAt = VietnamTime.Now;
        }

        if (changed)
            await _weeklyRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> WeekIsFilledAsync(
        int contractId,
        DateOnly weekMonday,
        CancellationToken cancellationToken = default)
    {
        var selection = await _weeklyRepository.GetByContractAndWeekAsync(
            contractId, weekMonday, cancellationToken: cancellationToken);
        return selection != null && ContractWeeklySelectionStatuses.IsFilled(selection.Status);
    }

    public async Task<ContractWeeklySelection> SaveWeeklySelectionAsync(
        Contract contract,
        int userId,
        DateOnly weekMonday,
        IReadOnlyDictionary<DateOnly, OrganizationMealOrderDraftDay> mergedDays,
        string status,
        string invoiceSuffix = "",
        CancellationToken cancellationToken = default)
    {
        if (!ContractWeeklySelectionStatuses.IsFilled(status)
            && !string.Equals(status, ContractWeeklySelectionStatuses.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Invalid weekly selection status: {status}");
        }

        await EnsureWeeksSeededAsync(contract, cancellationToken);

        var selection = await _weeklyRepository.GetByContractAndWeekAsync(
            contract.Id,
            weekMonday,
            includeItems: true,
            cancellationToken: cancellationToken);

        if (selection == null)
        {
            throw new ArgumentException(
                $"Tuần {weekMonday:dd/MM/yyyy} không có ngày phục vụ trong hợp đồng.");
        }

        if (ContractWeeklySelectionStatuses.IsFilled(selection.Status))
        {
            throw new ArgumentException(
                "Tuần này đã chọn món. Vui lòng xem thực đơn trong chi tiết hợp đồng.");
        }

        if (!contract.SourceOrderId.HasValue)
            throw new InvalidOperationException("Contract checkout is not complete.");

        var mealUnitPrice = contract.MealUnitPrice ?? 0m;
        var scheduledUtc = VietnamTime.CalendarDateMidnight(weekMonday);

        Order? weekOrder = null;
        if (selection.FulfillmentOrderId is int fulfillmentId)
            weekOrder = await _orderRepository.GetByIdAsync(fulfillmentId);

        weekOrder ??= await _orderRepository.GetContractWeekOrderAsync(
            contract.Id, weekMonday, cancellationToken);

        if (weekOrder == null)
        {
            weekOrder = new Order
            {
                UserId = userId,
                ContractId = contract.Id,
                OrderDate = VietnamTime.Now,
                ScheduledDate = scheduledUtc,
                Status = OrderLifecycleStatus.Confirmed,
                PaymentStatus = OrderPaymentStatus.Paid,
                SubtotalAmount = 0,
                DiscountAmount = 0,
                TotalAmount = 0,
                CreatedAt = VietnamTime.Now,
                InvoiceCode = $"Tuan-{weekMonday:yyyyMMdd}-{contract.Id}{invoiceSuffix}",
            };

            var source = await _orderRepository.GetByIdAsync(contract.SourceOrderId.Value);
            if (source != null)
            {
                OrganizationMealDeliveryValidator.ApplyToOrder(weekOrder, new OrganizationMealOrderDraftDelivery
                {
                    RecipientName = source.RecipientName ?? "",
                    RecipientPhone = source.RecipientPhone ?? "",
                    RecipientEmail = source.RecipientEmail ?? "",
                    DeliveryAddress = source.DeliveryAddress ?? "",
                    DeliveryWardDistrict = source.DeliveryWardDistrict,
                    DeliveryNotes = source.DeliveryNotes,
                    PreferredDeliveryTime = source.PreferredDeliveryTime,
                });
            }

            await _orderRepository.AddAsync(weekOrder, cancellationToken);
            await _orderRepository.CommitAsync();
            weekOrder = await _orderRepository.GetContractWeekOrderAsync(
                contract.Id, weekMonday, cancellationToken) ?? weekOrder;
        }
        else
        {
            weekOrder = await _orderRepository.GetContractWeekOrderAsync(
                contract.Id, weekMonday, cancellationToken) ?? weekOrder;
        }

        weekOrder.OrderItems.Clear();
        selection.Items.Clear();

        foreach (var day in mergedDays.Values.OrderBy(d => d.ServiceDate))
        {
            foreach (var line in day.Main)
            {
                weekOrder.OrderItems.Add(new OrderItem
                {
                    DishId = line.DishId,
                    Quantity = line.Quantity,
                    UnitPrice = mealUnitPrice,
                    TotalPrice = decimal.Round(mealUnitPrice * line.Quantity, 2, MidpointRounding.AwayFromZero),
                    ServiceDate = day.ServiceDate,
                });

                selection.Items.Add(new ContractWeeklySelectionItem
                {
                    ServiceDate = day.ServiceDate,
                    DishId = line.DishId,
                    Quantity = line.Quantity,
                });
            }
        }

        weekOrder.UpdatedAt = VietnamTime.Now;
        selection.Status = status;
        selection.SelectedAt = VietnamTime.Now;
        selection.FulfillmentOrderId = weekOrder.Id;
        selection.UpdatedAt = VietnamTime.Now;

        await _weeklyRepository.SaveChangesAsync(cancellationToken);
        await _orderRepository.CommitAsync();

        return selection;
    }
}
