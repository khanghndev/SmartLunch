using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetDetailedMealStatistics;

public class GetDetailedMealStatisticsQueryHandler : IRequestHandler<GetDetailedMealStatisticsQuery, GetDetailedMealStatisticsResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetDetailedMealStatisticsQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetDetailedMealStatisticsResponse> Handle(GetDetailedMealStatisticsQuery request, CancellationToken cancellationToken)
    {
        var data = await _orderRepository.GetDetailedMealStatisticsAsync(request.StartDate, request.EndDate, request.OrganizationId);

        return new GetDetailedMealStatisticsResponse
        {
            Data = data
        };
    }
}
