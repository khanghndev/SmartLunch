namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Ngày không cung cấp suất ăn trong thời hạn hợp đồng Period-Based.</summary>
public class ContractExcludedDate
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public DateOnly ExcludedDate { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Contract Contract { get; set; } = null!;
}
