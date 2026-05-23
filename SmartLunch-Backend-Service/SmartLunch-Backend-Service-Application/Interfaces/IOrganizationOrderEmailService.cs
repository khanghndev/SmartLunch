using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationOrderEmailService
{
    Task SendOrderConfirmationAsync(Order order, int depositPercent, CancellationToken cancellationToken = default);

    Task SendPaymentReminderAsync(Order order, CancellationToken cancellationToken = default);
}
