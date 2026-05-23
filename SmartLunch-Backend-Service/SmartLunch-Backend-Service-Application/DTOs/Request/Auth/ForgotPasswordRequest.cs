namespace SmartLunch.Backend.Service.Application.DTOs.Request.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;

    /// <summary>URL gốc trang đặt lại mật khẩu trên Web, ví dụ https://localhost:5103/Auth/ResetPassword</summary>
    public string? ResetPageUrl { get; set; }
}

public class ConfirmForgotPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
