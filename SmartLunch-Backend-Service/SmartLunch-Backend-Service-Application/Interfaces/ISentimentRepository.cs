using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ISentimentRepository
{
    Task<Sentiment?> GetByIdAsync(int id);
    Task<(List<Sentiment> Sentiments, int TotalCount)> GetSentimentsAsync(int page, int pageSize, string? searchTerm = null);
}
