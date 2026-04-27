using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ISystemLogRepository
{
    Task<(List<SystemLog> Logs, int TotalCount)> GetSystemLogsAsync(int page, int pageSize);
    Task<List<SystemLog>> GetAllAsync(int maxRows = 10000);
    Task<int> BulkInsertAsync(IEnumerable<SystemLog> logs);
}

