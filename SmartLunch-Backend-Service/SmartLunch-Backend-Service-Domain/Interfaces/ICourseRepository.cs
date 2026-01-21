using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId);
    Task<Course> CreateAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task<bool> DeleteAsync(Guid id);
}
