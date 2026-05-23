using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.API.Services;

/// <summary>Gửi email nhắc thanh toán cho đơn đã ký phụ lục nhưng chưa thanh toán.</summary>
public sealed class OrganizationPaymentReminderHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SmtpOptions _smtp;
    private readonly ILogger<OrganizationPaymentReminderHostedService> _logger;

    public OrganizationPaymentReminderHostedService(
        IServiceScopeFactory scopeFactory,
        IOptions<SmtpOptions> smtp,
        ILogger<OrganizationPaymentReminderHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _smtp = smtp.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pollMinutes = Math.Max(15, _smtp.PaymentReminderPollMinutes);
        var interval = TimeSpan.FromMinutes(pollMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Payment reminder job failed.");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        if (!_smtp.Enabled)
            return;

        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IOrganizationOrderEmailService>();

        var hours = Math.Max(1, _smtp.PaymentReminderIntervalHours);
        var cutoff = VietnamTime.Now.AddHours(-hours);

        var orders = await orderRepository.GetOrdersPendingPaymentReminderAsync(cutoff, 40, cancellationToken);
        foreach (var order in orders)
        {
            try
            {
                await emailService.SendPaymentReminderAsync(order, cancellationToken);
                await orderRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Payment reminder failed for order {OrderId}.", order.Id);
            }
        }
    }
}
