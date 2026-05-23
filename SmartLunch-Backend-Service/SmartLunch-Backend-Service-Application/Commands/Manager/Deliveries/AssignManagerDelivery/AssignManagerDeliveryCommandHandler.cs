using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.ManagerDeliveries;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.Manager.Deliveries.AssignManagerDelivery;

public class AssignManagerDeliveryCommandHandler : IRequestHandler<AssignManagerDeliveryCommand, ManagerDeliveryListItemDto>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUserRepository _userRepository;

    public AssignManagerDeliveryCommandHandler(IDeliveryRepository deliveryRepository, IUserRepository userRepository)
    {
        _deliveryRepository = deliveryRepository;
        _userRepository = userRepository;
    }

    public async Task<ManagerDeliveryListItemDto> Handle(AssignManagerDeliveryCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.ShipperUserId <= 0)
            throw new ArgumentException("ShipperUserId is required.");

        var shipper = await _userRepository.GetByIdAsync(request.Request.ShipperUserId);
        if (shipper == null || !shipper.IsActive)
            throw new ArgumentException("Shipper not found or inactive.");

        var isShipper = shipper.UserRoles.Any(ur =>
            ur.IsActive && string.Equals(ur.Role.Name, "Shipper", StringComparison.OrdinalIgnoreCase));
        if (!isShipper)
            throw new ArgumentException("Selected user is not a shipper.");

        var delivery = await _deliveryRepository.GetByIdWithOrderAsync(request.DeliveryId, cancellationToken);
        if (delivery == null)
            throw new KeyNotFoundException($"Delivery {request.DeliveryId} not found.");

        var status = (delivery.DeliveryStatus ?? "pending").Trim().ToLowerInvariant();
        if (status is "completed" or "failed" or "rejected")
            throw new InvalidOperationException("Cannot reassign a closed delivery.");

        delivery.AssignedStaffId = request.Request.ShipperUserId;
        if (status == "pending")
            delivery.DeliveryStatus = "received";

        if (!string.IsNullOrWhiteSpace(request.Request.Notes))
        {
            var note = request.Request.Notes.Trim();
            delivery.Notes = string.IsNullOrWhiteSpace(delivery.Notes)
                ? note
                : $"{delivery.Notes}\n{note}";
        }

        await _deliveryRepository.UpdateAsync(delivery, cancellationToken);
        await _deliveryRepository.CommitAsync(cancellationToken);

        var reloaded = await _deliveryRepository.GetByIdWithOrderAsync(request.DeliveryId, cancellationToken)
            ?? throw new InvalidOperationException("Delivery updated but failed to reload.");

        return ManagerDeliveryMapper.ToListItem(reloaded);
    }
}
