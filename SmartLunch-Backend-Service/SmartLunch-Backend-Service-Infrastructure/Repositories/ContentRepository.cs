using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ContentRepository : IContentRepository
{
    private readonly CourseDbContext _context;

    public ContentRepository(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<CourseContent?> GetByIdAsync(Guid id)
    {
        return await _context.CourseContents.FindAsync(id);
    }

    public async Task<IEnumerable<CourseContent>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.CourseContents
            .Where(c => c.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<CourseContent> CreateAsync(CourseContent content)
    {
        _context.CourseContents.Add(content);
        await _context.SaveChangesAsync();
        return content;
    }

    public async Task<CourseContent> UpdateAsync(CourseContent content)
    {
        _context.CourseContents.Update(content);
        await _context.SaveChangesAsync();
        return content;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var content = await _context.CourseContents.FindAsync(id);
        if (content == null) return false;

        _context.CourseContents.Remove(content);
        await _context.SaveChangesAsync();
        return true;
    }
}

