using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.ManagerDeliveries;

namespace SmartLunch.Backend.Service.Application.Queries.Manager.Deliveries.GetManagerDeliveries;

public class GetManagerDeliveriesQueryHandler : IRequestHandler<GetManagerDeliveriesQuery, GetManagerDeliveriesResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetManagerDeliveriesQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetManagerDeliveriesResponse> Handle(GetManagerDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

        var (deliveries, totalCount) = await _deliveryRepository.GetForManagerAsync(
            page, pageSize, request.Status, request.ScheduledOn, request.SearchTerm, request.UnassignedOnly, cancellationToken);

        var (pending, inProgress, completed, unassigned) =
            await _deliveryRepository.GetManagerStatsAsync(request.ScheduledOn, cancellationToken);

        return new GetManagerDeliveriesResponse
        {
            Data = deliveries.Select(ManagerDeliveryMapper.ToListItem).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Stats = new ManagerDeliveryStatsDto
            {
                Pending = pending,
                InProgress = inProgress,
                Completed = completed,
                Unassigned = unassigned,
            },
        };
    }
}
