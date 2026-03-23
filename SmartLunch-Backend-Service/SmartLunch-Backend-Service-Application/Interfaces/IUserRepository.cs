using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> IsEmailTakenByAnotherUserAsync(string email, Guid excludeUserId);
    Task<(List<User> Users, int TotalCount)> GetUsersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool? isActive = null,
        string? roleName = null);
}
