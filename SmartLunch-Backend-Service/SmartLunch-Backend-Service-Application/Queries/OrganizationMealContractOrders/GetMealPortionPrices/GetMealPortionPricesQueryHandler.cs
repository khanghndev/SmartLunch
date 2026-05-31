using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealContractOrders.GetMealPortionPrices;

public sealed class GetMealPortionPricesQueryHandler
    : IRequestHandler<GetMealPortionPricesQuery, GetMealPortionPricesResponse>
{
    private readonly IDishValueRepository _dishValues;

    public GetMealPortionPricesQueryHandler(IDishValueRepository dishValues)
    {
        _dishValues = dishValues;
    }

    public async Task<GetMealPortionPricesResponse> Handle(
        GetMealPortionPricesQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _dishValues.GetActiveOrderedAsync(cancellationToken);
        return new GetMealPortionPricesResponse
        {
            Items = rows.Select(v => new MealPortionPriceOptionDto
            {
                Id = v.Id,
                Amount = v.Amount,
                Label = v.Label,
                Code = v.Code,
            }).ToList(),
        };
    }
}
