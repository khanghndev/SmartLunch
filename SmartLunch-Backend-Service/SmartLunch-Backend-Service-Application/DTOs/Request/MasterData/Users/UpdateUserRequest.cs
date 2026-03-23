namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;

/// <summary>Cập nhật tài khoản — sửa thông tin, đặt mật khẩu mới, khóa/mở (IsActive).</summary>
public class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }

    /// <summary>Khóa (false) hoặc mở (true) đăng nhập.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Đặt lại mật khẩu (hash BCrypt); chỉ gửi khi cần đổi.</summary>
    public string? NewPassword { get; set; }
}
