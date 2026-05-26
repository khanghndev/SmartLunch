using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationOrderEmailService
{
    Task SendOrderConfirmationAsync(Order order, int depositPercent, CancellationToken cancellationToken = default);

    Task SendPaymentReminderAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>Nhắc đặt món cho tuần kế tiếp (HĐ Period-Based, tối thứ 4).</summary>
    Task SendWeeklyMealSelectionReminderAsync(
        Contract contract,
        DateOnly weekStart,
        CancellationToken cancellationToken = default);
}
