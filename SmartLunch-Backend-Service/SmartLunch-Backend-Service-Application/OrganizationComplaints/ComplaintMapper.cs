using System.Net;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

public static class ComplaintMapper
{
    private static readonly TimeSpan SignedUrlLifetime = TimeSpan.FromMinutes(30);

    public static async Task<string> ResolveStorageUrlAsync(IStorageService storage, string objectName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            return string.Empty;
        if (objectName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return objectName;

        var signed = await storage.CreateSignedUrlAsync(objectName, HttpMethod.Get, contentType: null, SignedUrlLifetime);
        return signed.Url;
    }

    public static async Task<ComplaintEvidenceDto> ToEvidenceDtoAsync(
        ComplaintEvidence evidence,
        IStorageService storage,
        CancellationToken cancellationToken) => new()
    {
        Id = evidence.Id,
        Kind = evidence.Kind,
        MediaType = evidence.MediaType,
        Url = await ResolveStorageUrlAsync(storage, evidence.StorageObjectName, cancellationToken),
        SortOrder = evidence.SortOrder,
        CreatedAt = evidence.CreatedAt,
    };

    public static OrganizationComplaintSummaryDto ToSummary(Complaint c) => new()
    {
        Id = c.Id,
        Code = c.Code,
        OrderId = c.OrderId,
        Title = c.Title,
        Reason = c.Reason,
        Status = c.Status,
        Resolution = c.Resolution,
        FinalRefundAmount = c.FinalRefundAmount,
        RefundPaymentId = c.RefundPaymentId,
        CreatedAt = c.CreatedAt,
        SubmittedAt = c.SubmittedAt,
        ComplaintDeadlineAt = c.ComplaintDeadlineAt,
    };

    public static async Task<OrganizationComplaintDetailDto> ToDetailAsync(
        Complaint c,
        IStorageService storage,
        CancellationToken cancellationToken)
    {
        var dto = new OrganizationComplaintDetailDto
        {
            Id = c.Id,
            Code = c.Code,
            OrderId = c.OrderId,
            Title = c.Title,
            Reason = c.Reason,
            Status = c.Status,
            Resolution = c.Resolution,
            FinalRefundAmount = c.FinalRefundAmount,
            RefundPaymentId = c.RefundPaymentId,
            CreatedAt = c.CreatedAt,
            SubmittedAt = c.SubmittedAt,
            ComplaintDeadlineAt = c.ComplaintDeadlineAt,
            Description = c.Description,
            MissingPortionCount = c.MissingPortionCount,
            RefundPortionCount = c.RefundPortionCount,
            SuggestedRefundAmount = c.SuggestedRefundAmount,
            ResolutionNote = c.ResolutionNote,
            ResolvedAt = c.ResolvedAt,
        };

        foreach (var e in c.Evidence.OrderBy(x => x.SortOrder).ThenBy(x => x.Id))
            dto.Evidence.Add(await ToEvidenceDtoAsync(e, storage, cancellationToken));

        return dto;
    }

    public static async Task<ManagerComplaintDetailDto> ToManagerDetailAsync(
        Complaint c,
        IStorageService storage,
        CancellationToken cancellationToken)
    {
        var baseDto = await ToDetailAsync(c, storage, cancellationToken);
        var order = c.Order;
        var delivery = order?.Deliveries?
            .Where(d => string.Equals(d.DeliveryStatus, "completed", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(d => d.DeliveredAt)
            .FirstOrDefault();

        DeliveryProofContextDto? shipper = null;
        if (delivery != null)
        {
            var proofUrl = delivery.ProofImageUrl ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(proofUrl) && !proofUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                proofUrl = await ResolveStorageUrlAsync(storage, proofUrl, cancellationToken);

            shipper = new DeliveryProofContextDto
            {
                DeliveryId = delivery.Id,
                DeliveredAt = delivery.DeliveredAt,
                ProofImageUrl = proofUrl,
                RecipientConfirmedName = delivery.RecipientConfirmedName,
                RecipientConfirmedAt = delivery.RecipientConfirmedAt,
                ShipperName = FormatPersonName(delivery.AssignedStaff?.FirstName, delivery.AssignedStaff?.LastName, delivery.AssignedStaff?.Username),
            };
        }

        return new ManagerComplaintDetailDto
        {
            Id = baseDto.Id,
            Code = baseDto.Code,
            OrderId = baseDto.OrderId,
            Title = baseDto.Title,
            Reason = baseDto.Reason,
            Status = baseDto.Status,
            Resolution = baseDto.Resolution,
            FinalRefundAmount = baseDto.FinalRefundAmount,
            RefundPaymentId = baseDto.RefundPaymentId,
            CreatedAt = baseDto.CreatedAt,
            SubmittedAt = baseDto.SubmittedAt,
            ComplaintDeadlineAt = baseDto.ComplaintDeadlineAt,
            Description = baseDto.Description,
            MissingPortionCount = baseDto.MissingPortionCount,
            RefundPortionCount = baseDto.RefundPortionCount,
            SuggestedRefundAmount = baseDto.SuggestedRefundAmount,
            ResolutionNote = baseDto.ResolutionNote,
            ResolvedAt = baseDto.ResolvedAt,
            Evidence = baseDto.Evidence,
            OrganizationName = order?.Contract?.Organization?.Name,
            OrderInvoiceCode = order?.InvoiceCode,
            OrderTotalAmount = order?.TotalAmount ?? 0,
            MealCount = order?.OrderItems?.Sum(i => i.Quantity) ?? 0,
            UnitPricePerPortion = order != null ? ComplaintRefundCalculator.GetUnitPricePerPortion(order) : 0,
            ShipperDelivery = shipper,
            ComplainantName = FormatPersonName(c.User?.FirstName, c.User?.LastName, c.User?.Username),
        };
    }

    private static string? FormatPersonName(string? first, string? last, string? fallback)
    {
        var name = $"{first} {last}".Trim();
        return string.IsNullOrWhiteSpace(name) ? fallback : name;
    }
}
