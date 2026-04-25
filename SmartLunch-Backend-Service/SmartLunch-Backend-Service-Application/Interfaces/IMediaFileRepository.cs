using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IMediaFileRepository
{
    Task<MediaFile?> GetByIdAsync(int id);
    Task<MediaFile?> GetByIdForOwnerAsync(int id, int ownerUserId);
    Task<(List<MediaFile> MediaFiles, int TotalCount)> GetMediaFilesAsync(int page, int pageSize, string? searchTerm = null);
    Task<MediaFile> CreateAsync(MediaFile mediaFile);
}
