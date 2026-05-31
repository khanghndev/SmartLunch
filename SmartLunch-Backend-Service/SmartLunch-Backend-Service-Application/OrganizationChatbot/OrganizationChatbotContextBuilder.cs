using System.Globalization;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public sealed class OrganizationChatbotContextBuilder : IOrganizationChatbotContextBuilder
{
    private readonly IUserOrganizationRepository _userOrganizations;
    private readonly IOrganizationRepository _organizations;
    private readonly IContractRepository _contracts;
    private readonly IOrderRepository _orders;
    private readonly IWeeklyMenuRepository _weeklyMenus;
    private readonly ICustomerTypeRepository _customerTypes;
    private readonly IComplaintRepository _complaints;
    private readonly IPartnerRepository _partners;

    public OrganizationChatbotContextBuilder(
        IUserOrganizationRepository userOrganizations,
        IOrganizationRepository organizations,
        IContractRepository contracts,
        IOrderRepository orders,
        IWeeklyMenuRepository weeklyMenus,
        ICustomerTypeRepository customerTypes,
        IComplaintRepository complaints,
        IPartnerRepository partners)
    {
        _userOrganizations = userOrganizations;
        _organizations = organizations;
        _contracts = contracts;
        _orders = orders;
        _weeklyMenus = weeklyMenus;
        _customerTypes = customerTypes;
        _complaints = complaints;
        _partners = partners;
    }

    public async Task<OrganizationChatbotKnowledgePack> BuildAsync(int userId, CancellationToken cancellationToken = default)
    {
        var now = VietnamTime.Now;
        var pack = new OrganizationChatbotKnowledgePack { GeneratedAt = now };

        var memberships = (await _userOrganizations.GetActiveByUserIdAsync(userId)).ToList();
        var orgIds = memberships.Select(m => m.OrganizationId).Distinct().ToList();

        Organization? org = orgIds.Count > 0 ? await _organizations.GetByIdAsync(orgIds[0]) : null;
        if (org != null)
        {
            pack.Organization = new OrganizationKnowledge
            {
                Name = org.Name,
                Type = OrganizationChatbotLabels.FormatOrganizationType(org.Type),
                TypeKey = org.Type,
                Address = org.Address,
                Phone = org.Phone,
                ContactPerson = org.ContactPerson,
                ContactEmail = org.ContactEmail,
                TaxCode = org.TaxCode,
            };
            pack.CutoffRulesText = OrganizationChatbotLabels.GetCutoffRuleText(org.Type);
        }

        var allContracts = orgIds.Count > 0
            ? await _contracts.GetByOrganizationIdsAsync(orgIds, cancellationToken)
            : new List<Contract>();

        var activeContracts = allContracts
            .Where(c => string.Equals(c.Status, ContractStatus.Active, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(c => c.StartDate)
            .ToList();

        pack.Contracts = activeContracts.Select(c => new ContractKnowledge
        {
            Code = c.Code ?? $"HĐ-{c.Id}",
            ContractNumber = c.ContractNumber,
            ContractType = c.ContractType,
            Status = c.Status,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            MealUnitPriceVnd = c.MealUnitPrice,
            MealsPerDay = c.MealsPerDay,
            IsDigitallySigned = c.IsDigitallySigned,
        }).ToList();

        var (orderRows, totalOrders) = await _orders.GetOrdersAsync(1, 30, restrictToUserId: userId);
        var orderKnowledge = new List<OrderKnowledge>();

        foreach (var o in orderRows)
        {
            if (string.Equals(o.Status, OrderLifecycleStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
                continue;

            var detailed = await _orders.GetByIdWithDetailsAsync(o.Id);
            if (detailed == null) continue;

            var delivery = detailed.Deliveries?.OrderByDescending(d => d.CreatedAt).FirstOrDefault();
            orderKnowledge.Add(new OrderKnowledge
            {
                Id = detailed.Id,
                Code = detailed.Code ?? $"#{detailed.Id}",
                InvoiceCode = detailed.InvoiceCode,
                ScheduledDate = detailed.ScheduledDate,
                Status = detailed.Status,
                StatusVi = OrganizationChatbotLabels.FormatOrderStatus(detailed.Status),
                PaymentStatus = detailed.PaymentStatus,
                PaymentStatusVi = OrganizationChatbotLabels.FormatPaymentStatus(detailed.PaymentStatus),
                TotalAmountVnd = detailed.TotalAmount,
                MainPortionCount = OrganizationComplaintRules.CountMainPortions(detailed),
                DeliveryStatus = delivery?.DeliveryStatus,
                DeliveredAt = delivery?.DeliveredAt,
                ComplaintDeadlineAt = OrganizationComplaintRules.GetComplaintDeadline(detailed),
                DishNames = detailed.OrderItems?
                    .Where(i => i.Dish != null)
                    .Select(i => $"{i.Dish!.Name} x{i.Quantity}")
                    .Take(8)
                    .ToList() ?? new List<string>(),
            });
        }

        pack.Orders = orderKnowledge;
        pack.PendingPaymentOrders = orderKnowledge
            .Where(o => OrganizationChatbotLabels.IsAwaitingPayment(o.PaymentStatus))
            .ToList();

        pack.Summary = new AccountSummary
        {
            TotalOrders = totalOrders,
            ActiveContracts = activeContracts.Count,
            OrdersNeedingPayment = pack.PendingPaymentOrders.Count,
            TotalOutstandingVnd = pack.PendingPaymentOrders.Sum(o => o.TotalAmountVnd),
            UpcomingOrders = orderKnowledge.Count(o => o.ScheduledDate.Date >= now.Date),
            OpenComplaints = 0,
        };

        var (complaints, _) = await _complaints.GetComplaintsAsync(1, 10, userId: userId, cancellationToken: cancellationToken);
        pack.Complaints = complaints.Select(c => new ComplaintKnowledge
        {
            Title = c.Title,
            Status = c.Status,
            StatusVi = OrganizationChatbotLabels.FormatComplaintStatus(c.Status),
            CreatedAt = c.CreatedAt,
            OrderCode = c.OrderId?.ToString(),
        }).ToList();
        pack.Summary.OpenComplaints = complaints.Count;

        pack.WeeklyMenu = await BuildWeeklyMenuForDateAsync(org?.Type, now, "current", cancellationToken);
        var nextWeekRef = OrganizationChatbotWeekHelper.GetMonday(now).AddDays(7).AddDays(2);
        pack.WeeklyMenuNext = await BuildWeeklyMenuForDateAsync(org?.Type, nextWeekRef, "next", cancellationToken)
                              ?? OrganizationChatbotMenuFormatter.ProjectNextWeekFromCurrent(pack.WeeklyMenu);

        if (activeContracts.Count > 0)
        {
            var partner = await _partners.GetByIdAsync(activeContracts[0].PartnerId);
            pack.SupportContact = new SupportContactKnowledge
            {
                SupplierName = partner?.LegalName,
                Hotline = partner?.Phone ?? "0334 297 551",
                Email = partner?.Email ?? "contact@HuitMeal.com",
                Address = partner?.Address,
            };
        }
        else
        {
            pack.SupportContact = new SupportContactKnowledge
            {
                Hotline = "0334 297 551",
                Email = "contact@HuitMeal.com",
            };
        }

        pack.Policies = new Dictionary<string, string>
        {
            ["complaintWindowHours"] = "24",
            ["complaintWindowDescription"] = "Khiếu nại trong 24 giờ sau khi đơn chuyển sang Đã giao",
            ["paymentMethod"] = "PayOS trên trang chi tiết đơn sau khi ký phụ lục",
            ["cutoffRules"] = pack.CutoffRulesText,
            ["howToOrder"] = OrganizationChatbotLabels.GetOrderHowToText(org?.Type ?? "office", org?.Name),
        };

        return pack;
    }

    private async Task<WeeklyMenuKnowledge?> BuildWeeklyMenuForDateAsync(
        string? organizationType,
        DateTime referenceDate,
        string periodKey,
        CancellationToken cancellationToken)
    {
        var customerTypes = await _customerTypes.GetAllAsync(cancellationToken);
        var customerTypeId = OrganizationChatbotLabels.ResolveCustomerTypeId(organizationType, customerTypes);

        var menu = await _weeklyMenus.GetWeeklyMenuWithSchedulesByDateAsync(referenceDate, customerTypeId)
                   ?? await _weeklyMenus.GetWeeklyMenuWithSchedulesByDateAsync(referenceDate, customerTypeId: null);

        if (menu?.MenuSchedules == null || menu.MenuSchedules.Count == 0)
            return null;

        var mapped = OrganizationChatbotMenuFormatter.MapWeeklyMenu(menu);
        if (mapped != null)
            mapped.PeriodKey = periodKey;
        return mapped;
    }
}
