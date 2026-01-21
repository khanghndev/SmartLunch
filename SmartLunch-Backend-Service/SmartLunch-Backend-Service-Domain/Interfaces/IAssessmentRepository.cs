using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IAssessmentRepository
{
    Task<Assessment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Assessment>> GetByCourseIdAsync(Guid courseId);
    Task<Assessment> CreateAsync(Assessment assessment);
    Task<Assessment> UpdateAsync(Assessment assessment);
    Task<bool> DeleteAsync(Guid id);
}
