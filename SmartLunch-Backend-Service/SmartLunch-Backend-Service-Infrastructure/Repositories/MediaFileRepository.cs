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

    public async Task<(List<MediaFile> MediaFiles, int TotalCount)> GetMediaFilesAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.MediaFiles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.Bucket.Contains(searchTerm) ||
                x.ObjectName.Contains(searchTerm) ||
                (x.OriginalFileName != null && x.OriginalFileName.Contains(searchTerm)) ||
                x.ContentType.Contains(searchTerm) ||
                (x.Md5HashBase64 != null && x.Md5HashBase64.Contains(searchTerm)) ||
                x.MediaType.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var mediaFiles = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (mediaFiles, totalCount);
    }

    public async Task<MediaFile> CreateAsync(MediaFile mediaFile)
    {
        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync();
        return mediaFile;
    }
}
