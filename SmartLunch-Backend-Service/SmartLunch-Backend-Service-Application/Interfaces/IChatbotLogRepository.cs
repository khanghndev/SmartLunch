using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IChatbotLogRepository
{
    Task<ChatbotLog?> GetByIdAsync(Guid id);
    Task<(List<ChatbotLog> ChatbotLogs, int TotalCount)> GetChatbotLogsAsync(int page, int pageSize, string? searchTerm = null);
}
