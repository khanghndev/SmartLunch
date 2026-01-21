using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IContentRepository
{
    Task<CourseContent?> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseContent>> GetByCourseIdAsync(Guid courseId);
    Task<CourseContent> CreateAsync(CourseContent content);
    Task<CourseContent> UpdateAsync(CourseContent content);
    Task<bool> DeleteAsync(Guid id);
}
