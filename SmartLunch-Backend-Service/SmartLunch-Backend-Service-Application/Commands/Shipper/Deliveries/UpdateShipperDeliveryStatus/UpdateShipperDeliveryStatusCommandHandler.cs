using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Shipper.Deliveries.UpdateShipperDeliveryStatus;

public class UpdateShipperDeliveryStatusCommandHandler
    : IRequestHandler<UpdateShipperDeliveryStatusCommand, GetShipperDeliveryResponse>
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "received",
        "in_transit",
        "completed",
        "failed",
        "rejected"
    };

    private readonly IDeliveryRepository _deliveryRepository;

    public UpdateShipperDeliveryStatusCommandHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetShipperDeliveryResponse> Handle(UpdateShipperDeliveryStatusCommand request, CancellationToken cancellationToken)
    {
        var target = (request.Request.Status ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(target))
            throw new ArgumentException("Status is required.");
        if (!AllowedStatuses.Contains(target))
            throw new ArgumentException("Invalid status. Allowed values: received, in_transit, completed, failed, rejected.");

        if ((target == "failed" || target == "rejected") && string.IsNullOrWhiteSpace(request.Request.Notes))
            throw new ArgumentException("Notes is required when status is failed or rejected.");

        var delivery = await _deliveryRepository.GetByIdWithOrderAsync(request.DeliveryId, cancellationToken);
        if (delivery == null)
            throw new KeyNotFoundException($"Delivery with ID {request.DeliveryId} was not found.");

        // If unassigned pending, shipper can "receive" it (assign to themselves)
        if (!delivery.AssignedStaffId.HasValue)
        {
            if (!string.Equals(delivery.DeliveryStatus, "pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("This delivery is not available to claim.");

            if (target != "received" && target != "rejected")
                throw new InvalidOperationException("You must receive or reject the delivery first.");

            delivery.AssignedStaffId = request.ShipperUserId;
        }
        else if (delivery.AssignedStaffId.Value != request.ShipperUserId)
        {
            throw new KeyNotFoundException("Delivery was not found.");
        }

        var current = (delivery.DeliveryStatus ?? string.Empty).Trim().ToLowerInvariant();
        if (!IsAllowedTransition(current, target))
            throw new InvalidOperationException($"Cannot change delivery status from '{current}' to '{target}'.");

        if (target == "completed")
            throw new InvalidOperationException(
                "Không thể đánh dấu hoàn tất qua API trạng thái. Hãy dùng POST /proof (ảnh + chữ ký người nhận).");

        if (current != target)
        {
            delivery.DeliveryStatus = target;

            if (target is "failed" or "rejected")
                delivery.Notes = request.Request.Notes?.Trim();
        }

        await _deliveryRepository.UpdateAsync(delivery, cancellationToken);
        await _deliveryRepository.CommitAsync(cancellationToken);

        var reloaded = await _deliveryRepository.GetByIdWithOrderAsync(request.DeliveryId, cancellationToken)
            ?? throw new InvalidOperationException("Delivery updated but failed to reload.");

        var mealCount = reloaded.Order?.OrderItems?.Sum(i => i.Quantity) ?? 0;
        return new GetShipperDeliveryResponse
        {
            Delivery = MapDetail(reloaded, mealCount)
        };
    }

    internal static ShipperDeliveryDetailDto MapDetail(Domain.Entities.Delivery delivery, int mealCount) =>
        new()
        {
            DeliveryId = delivery.Id,
            OrderId = delivery.OrderId,
            DeliveryAddress = delivery.DeliveryAddress,
            DeliveryStatus = delivery.DeliveryStatus,
            ScheduledDateUtc = delivery.Order?.ScheduledDate ?? DateTime.MinValue,
            MealCount = mealCount,
            DeliveredAtUtc = delivery.DeliveredAt,
            ProofImageUrl = delivery.ProofImageUrl,
            ProofCapturedAtUtc = delivery.ProofCapturedAt,
            Notes = delivery.Notes,
            RecipientConfirmedName = delivery.RecipientConfirmedName,
            RecipientConfirmedAtUtc = delivery.RecipientConfirmedAt,
            RecipientSignatureUrl = delivery.RecipientSignatureUrl,
            ShipperSignatureUrl = delivery.ShipperSignatureUrl,
            HandoverDocumentUrl = delivery.HandoverDocumentUrl,
            RequiresRecipientSignature = string.Equals(
                delivery.DeliveryStatus, "in_transit", StringComparison.OrdinalIgnoreCase),
        };

    private static bool IsAllowedTransition(string current, string target)
    {
        if (string.Equals(current, target, StringComparison.OrdinalIgnoreCase))
            return true;

        if (current is "completed" or "failed" or "rejected")
            return false;

        return (current, target) switch
        {
            ("pending", "received") => true,
            ("pending", "rejected") => true,
            ("received", "in_transit") => true,
            ("received", "failed") => true,
            ("in_transit", "completed") => true,
            ("in_transit", "failed") => true,
            _ => false
        };
    }
}
