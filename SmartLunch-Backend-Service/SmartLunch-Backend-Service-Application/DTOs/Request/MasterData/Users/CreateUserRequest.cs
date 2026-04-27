namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;

/// <summary>Tạo tài khoản (Admin).</summary>
public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Gán một role ban đầu (tùy chọn); phân quyền chi tiết qua UserRole / UserPermission.</summary>
    public int? InitialRoleId { get; set; }
}
