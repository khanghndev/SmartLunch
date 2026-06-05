using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationOrderEmailService
{
    Task SendOrderConfirmationAsync(Order order, int depositPercent, CancellationToken cancellationToken = default);

    Task SendPaymentReminderAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>Nhắc đặt món trước 3 ngày tuần đang mở (HĐ Period-Based).</summary>
    Task SendWeeklyMealSelectionReminderAsync(
        Contract contract,
        DateOnly weekStart,
        CancellationToken cancellationToken = default);

    /// <summary>Thông báo hệ thống đã tự chọn món khi quá hạn.</summary>
    Task SendWeeklyMealAutoFilledAsync(
        Contract contract,
        DateOnly weekStart,
        CancellationToken cancellationToken = default);
}
