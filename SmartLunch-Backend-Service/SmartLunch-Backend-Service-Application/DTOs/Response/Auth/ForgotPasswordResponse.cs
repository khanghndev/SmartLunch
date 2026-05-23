namespace SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

public class ForgotPasswordResponse
{
    public string Message { get; set; } = "If the email exists, a reset link has been sent.";
}

public class ConfirmForgotPasswordResponse
{
    public int UserId { get; set; }
    public DateTime ChangedAt { get; set; }
}
