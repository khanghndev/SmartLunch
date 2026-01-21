using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class AssessmentRepository : IAssessmentRepository
{
    private readonly CourseDbContext _context;

    public AssessmentRepository(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<Assessment?> GetByIdAsync(Guid id)
    {
        return await _context.Assessments
            .Include(a => a.Course)
            .ThenInclude(c => c.Enrollments)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Assessment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Assessments
            .Where(a => a.CourseId == courseId)
            .Include(a => a.Course)
            .ThenInclude(c => c.Enrollments)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Assessment> CreateAsync(Assessment assessment)
    {
        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();
        return assessment;
    }

    public async Task<Assessment> UpdateAsync(Assessment assessment)
    {
        _context.Assessments.Update(assessment);
        await _context.SaveChangesAsync();
        return assessment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var assessment = await _context.Assessments.FindAsync(id);
        if (assessment == null) return false;

        _context.Assessments.Remove(assessment);
        await _context.SaveChangesAsync();
        return true;
    }
}
