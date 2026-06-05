using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Infrastructure.Email;

public sealed class OrganizationOrderEmailService : IOrganizationOrderEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly IOrganizationMealDocumentPdfService _documentPdfService;
    private readonly IContractPdfService _contractPdfService;
    private readonly IContractRepository _contractRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrganizationOrderEmailService> _logger;

    public OrganizationOrderEmailService(
        IEmailSender emailSender,
        IOrganizationMealDocumentPdfService documentPdfService,
        IContractPdfService contractPdfService,
        IContractRepository contractRepository,
        IOrderRepository orderRepository,
        ILogger<OrganizationOrderEmailService> logger)
    {
        _emailSender = emailSender;
        _documentPdfService = documentPdfService;
        _contractPdfService = contractPdfService;
        _contractRepository = contractRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(
        Order order,
        int depositPercent,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(order.RecipientEmail))
            return;

        if (order.OrderConfirmationEmailSentAt.HasValue)
            return;

        var contract = order.Contract;
        if (contract?.Partner == null || contract.Organization == null)
        {
            _logger.LogWarning("Order {OrderId} missing contract/partner/org for confirmation PDF.", order.Id);
            return;
        }

        var org = contract.Organization;
        var partner = contract.Partner;
        var buyerName = ResolveBuyerDisplayName(order, org);
        var depositAmount = ResolveDepositAmount(order, depositPercent);
        var fullAddress = BuildFullAddress(order);

        byte[] pdfBytes;
        try
        {
            await RegenerateStoredContractPdfAsync(contract, partner, org, order, cancellationToken);
            pdfBytes = _documentPdfService.GenerateCombinedPdfBytes(
                contract,
                partner,
                org,
                order,
                buyerName,
                signatureDataUrl: null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Combined PDF failed for order {OrderId}; fallback to contract-only.", order.Id);
            pdfBytes = _contractPdfService.GenerateContractPdfBytes(
                contract,
                partner,
                org,
                order,
                OrganizationMealDeliveryPdfContext.FromOrder(order));
        }

        var html = OrganizationEmailHtmlTemplates.OrderConfirmation(
            order,
            org.Name,
            contract.ContractNumber,
            depositPercent,
            depositAmount,
            fullAddress);

        var invoice = order.InvoiceCode ?? order.Id.ToString();
        var attachments = new List<EmailAttachment>();
        if (pdfBytes.Length > 0)
        {
            attachments.Add(new EmailAttachment
            {
                FileName = $"Hop-dong-phu-luc-{invoice}.pdf",
                Content = pdfBytes,
                ContentType = "application/pdf",
            });
        }

        await _emailSender.SendAsync(
            order.RecipientEmail,
            $"[HuitMeal] Xác nhận đơn đặt suất {invoice} — Hợp đồng & phụ lục",
            html,
            attachments,
            cancellationToken);

        order.OrderConfirmationEmailSentAt = VietnamTime.Now;
    }

    public async Task SendPaymentReminderAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(order.RecipientEmail))
            return;

        if (order.PaymentReminderSentAt.HasValue)
            return;

        if (!string.Equals(order.PaymentStatus, OrderPaymentStatus.AwaitingPayment, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(order.PaymentStatus, OrderPaymentStatus.Unpaid, StringComparison.OrdinalIgnoreCase))
            return;

        if (!order.AnnexSignedAt.HasValue)
            return;

        var orgName = order.Contract?.Organization?.Name ?? "Đơn vị của bạn";
        var depositPercent = InferDepositPercent(order);
        var depositAmount = ResolveDepositAmount(order, depositPercent);

        var html = OrganizationEmailHtmlTemplates.PaymentReminder(
            order,
            orgName,
            depositAmount,
            depositPercent);

        var invoice = order.InvoiceCode ?? order.Id.ToString();
        await _emailSender.SendAsync(
            order.RecipientEmail,
            $"[HuitMeal] Nhắc thanh toán đặt cọc — {invoice}",
            html,
            cancellationToken: cancellationToken);

        order.PaymentReminderSentAt = VietnamTime.Now;
    }

    public async Task SendWeeklyMealSelectionReminderAsync(
        Contract contract,
        DateOnly weekStart,
        CancellationToken cancellationToken = default)
    {
        if (!contract.SourceOrderId.HasValue)
            return;

        var sourceOrder = await _orderRepository.GetByIdAsync(contract.SourceOrderId.Value);
        if (string.IsNullOrWhiteSpace(sourceOrder?.RecipientEmail))
            return;

        var orgName = contract.Organization?.Name ?? "Đơn vị của bạn";
        var weekEnd = weekStart.AddDays(6);
        var html = OrganizationEmailHtmlTemplates.WeeklyMealSelectionReminder(
            orgName,
            contract.ContractNumber,
            weekStart,
            weekEnd);

        await _emailSender.SendAsync(
            sourceOrder.RecipientEmail,
            $"[HuitMeal] Nhắc đặt món tuần {weekStart:dd/MM} – {weekEnd:dd/MM}",
            html,
            cancellationToken: cancellationToken);
    }

    public async Task SendWeeklyMealAutoFilledAsync(
        Contract contract,
        DateOnly weekStart,
        CancellationToken cancellationToken = default)
    {
        if (!contract.SourceOrderId.HasValue)
            return;

        var sourceOrder = await _orderRepository.GetByIdAsync(contract.SourceOrderId.Value);
        if (string.IsNullOrWhiteSpace(sourceOrder?.RecipientEmail))
            return;

        var orgName = contract.Organization?.Name ?? "Đơn vị của bạn";
        var weekEnd = weekStart.AddDays(6);
        var html = OrganizationEmailHtmlTemplates.WeeklyMealAutoFilled(
            orgName,
            contract.ContractNumber,
            weekStart,
            weekEnd);

        await _emailSender.SendAsync(
            sourceOrder.RecipientEmail,
            $"[HuitMeal] Đã tự chọn món tuần {weekStart:dd/MM} – {weekEnd:dd/MM}",
            html,
            cancellationToken: cancellationToken);
    }

    private async Task RegenerateStoredContractPdfAsync(
        Contract contract,
        Partner partner,
        Organization org,
        Order order,
        CancellationToken cancellationToken)
    {
        try
        {
            var url = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
                contract,
                partner,
                org,
                order,
                OrganizationMealDeliveryPdfContext.FromOrder(order),
                cancellationToken: cancellationToken);

            contract.ContractFileUrl = url;
            contract.UpdatedAt = VietnamTime.Now;
            await _contractRepository.UpdateAsync(contract);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not refresh stored contract PDF for contract {ContractId}.", contract.Id);
        }
    }

    private static string ResolveBuyerDisplayName(Order order, Organization org)
    {
        if (!string.IsNullOrWhiteSpace(order.RecipientName))
            return order.RecipientName.Trim();
        return org.Name;
    }

    private static decimal ResolveDepositAmount(Order order, int depositPercent)
    {
        var pending = order.Payments.FirstOrDefault(p =>
            string.Equals(p.Method, "payos", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase));

        if (pending != null)
            return pending.Amount;

        return decimal.Round(order.TotalAmount * (depositPercent / 100m), 0, MidpointRounding.AwayFromZero);
    }

    private static int InferDepositPercent(Order order)
    {
        var pending = order.Payments.FirstOrDefault(p =>
            string.Equals(p.Method, "payos", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase));

        if (pending != null && order.TotalAmount > 0)
        {
            var pct = (int)Math.Round(pending.Amount / order.TotalAmount * 100m, MidpointRounding.AwayFromZero);
            if (pct is >= 20 and <= 50)
                return pct;
        }

        return 30;
    }

    private static string BuildFullAddress(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
            return "—";

        var draft = new OrganizationMealOrderDraftDelivery
        {
            DeliveryAddress = order.DeliveryAddress,
            DeliveryWardDistrict = order.DeliveryWardDistrict,
        };
        return OrganizationMealDeliveryValidator.BuildFullAddress(draft);
    }
}
