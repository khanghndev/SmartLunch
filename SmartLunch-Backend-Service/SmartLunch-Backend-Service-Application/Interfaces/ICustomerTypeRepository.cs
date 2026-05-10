using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ICustomerTypeRepository
{
    Task<List<CustomerType>> GetAllAsync(CancellationToken cancellationToken = default);
}

