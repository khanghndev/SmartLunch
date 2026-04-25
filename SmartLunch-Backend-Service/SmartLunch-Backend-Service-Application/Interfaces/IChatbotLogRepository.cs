using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IChatbotLogRepository
{
    Task<ChatbotLog?> GetByIdAsync(int id);
    Task<(List<ChatbotLog> ChatbotLogs, int TotalCount)> GetChatbotLogsAsync(int page, int pageSize, string? searchTerm = null);
}
