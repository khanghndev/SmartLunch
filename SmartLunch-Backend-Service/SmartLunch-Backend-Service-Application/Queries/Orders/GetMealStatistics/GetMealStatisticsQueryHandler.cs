using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetMealStatistics;

public class GetMealStatisticsQueryHandler : IRequestHandler<GetMealStatisticsQuery, GetMealStatisticsResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetMealStatisticsQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetMealStatisticsResponse> Handle(GetMealStatisticsQuery request, CancellationToken cancellationToken)
    {
        var data = await _orderRepository.GetMealStatisticsAsync(request.StartDate, request.EndDate, request.UnitId);

        return new GetMealStatisticsResponse
        {
            Data = data
        };
    }
}
