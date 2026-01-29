using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IMediaFileRepository
{
    Task<MediaFile?> GetByIdAsync(Guid id);
    Task<MediaFile?> GetByIdForOwnerAsync(Guid id, Guid ownerUserId);
    Task<MediaFile> CreateAsync(MediaFile mediaFile);
}

