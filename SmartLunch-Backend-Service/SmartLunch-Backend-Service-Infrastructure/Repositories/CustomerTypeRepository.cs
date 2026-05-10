using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class CustomerTypeRepository : ICustomerTypeRepository
{
    private readonly SmartLunchDBContext _context;

    public CustomerTypeRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public Task<List<CustomerType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.CustomerTypes
            .AsNoTracking()
            .OrderBy(ct => ct.Id)
            .ToListAsync(cancellationToken);
    }
}

