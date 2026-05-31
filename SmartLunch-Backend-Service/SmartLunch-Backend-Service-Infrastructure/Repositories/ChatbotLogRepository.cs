using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ChatbotLogRepository : IChatbotLogRepository
{
    private readonly SmartLunchDBContext _context;

    public ChatbotLogRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<ChatbotLog?> GetByIdAsync(int id)
    {
        return await _context.ChatbotLogs
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<ChatbotLog> ChatbotLogs, int TotalCount)> GetChatbotLogsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.ChatbotLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Message.Contains(searchTerm) ||
                (e.Response != null && e.Response.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var chatbotLogs = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (chatbotLogs, totalCount);
    }

    public async Task<ChatbotLog> CreateAsync(ChatbotLog log, CancellationToken cancellationToken = default)
    {
        _context.ChatbotLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
        return log;
    }
}
