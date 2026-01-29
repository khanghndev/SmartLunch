using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class MediaFileRepository : IMediaFileRepository
{
    private readonly SmartLunchDBContext _context;

    public MediaFileRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<MediaFile?> GetByIdAsync(Guid id)
    {
        return await _context.MediaFiles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<MediaFile?> GetByIdForOwnerAsync(Guid id, Guid ownerUserId)
    {
        return await _context.MediaFiles.FirstOrDefaultAsync(x => x.Id == id && x.OwnerUserId == ownerUserId);
    }

    public async Task<MediaFile> CreateAsync(MediaFile mediaFile)
    {
        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync();
        return mediaFile;
    }
}

