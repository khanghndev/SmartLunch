using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.CustomerTypes;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerTypes.GetCustomerTypes;

public class GetCustomerTypesQueryHandler : IRequestHandler<GetCustomerTypesQuery, GetCustomerTypesResponse>
{
    private readonly ICustomerTypeRepository _repo;

    public GetCustomerTypesQueryHandler(ICustomerTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<GetCustomerTypesResponse> Handle(GetCustomerTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return new GetCustomerTypesResponse
        {
            Data = list.Select(ct => new CustomerTypeDto
            {
                Id = ct.Id,
                Code = ct.Code,
                ProfileKey = ct.ProfileKey,
                Name = ct.Name,
                Description = ct.Description
            }).ToList()
        };
    }
}

