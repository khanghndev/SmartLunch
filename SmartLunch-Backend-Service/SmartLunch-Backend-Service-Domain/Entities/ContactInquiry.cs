namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Yêu cầu liên hệ từ trang web (không bắt buộc đăng nhập).</summary>
public class ContactInquiry
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string InterestedService { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string Status { get; set; } = "pending";
    public string? ManagerReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public int? RepliedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual User? User { get; set; }
    public virtual User? RepliedByUser { get; set; }
}
