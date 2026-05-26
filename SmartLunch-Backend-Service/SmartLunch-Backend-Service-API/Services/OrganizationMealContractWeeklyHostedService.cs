using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.API.Services;

/// <summary>
/// Thứ 5 tối: nhắc đặt món tuần kế tiếp. Thứ 6 tối: auto random món chính nếu chưa có order_item.
/// </summary>
public sealed class OrganizationMealContractWeeklyHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SmtpOptions _smtp;
    private readonly ILogger<OrganizationMealContractWeeklyHostedService> _logger;

    public OrganizationMealContractWeeklyHostedService(
        IServiceScopeFactory scopeFactory,
        IOptions<SmtpOptions> smtp,
        ILogger<OrganizationMealContractWeeklyHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _smtp = smtp.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessWeeklyJobsAsync(stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Period contract weekly job failed.");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task ProcessWeeklyJobsAsync(CancellationToken cancellationToken)
    {
        if (!_smtp.Enabled)
            return;

        var vnNow = VietnamTime.Now;
        if (vnNow.Hour < 18 || vnNow.Hour >= 20)
            return;

        var day = vnNow.DayOfWeek;
        if (day is not DayOfWeek.Thursday and not DayOfWeek.Friday)
            return;

        using var scope = _scopeFactory.CreateScope();
        var contractRepository = scope.ServiceProvider.GetRequiredService<IContractRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IOrganizationOrderEmailService>();
        var weeklyJob = scope.ServiceProvider.GetRequiredService<OrganizationMealContractWeeklyJobService>();

        var today = DateOnly.FromDateTime(vnNow);
        var nextMonday = OrganizationMealPeriodContractCalculator.GetWeekMonday(today).AddDays(7);

        var contracts = await contractRepository.GetActivePeriodBasedContractsAsync(cancellationToken);

        if (day == DayOfWeek.Thursday)
        {
            foreach (var contract in contracts)
            {
                if (contract.LastWeeklyReminderWeekStart == nextMonday)
                    continue;

                try
                {
                    await emailService.SendWeeklyMealSelectionReminderAsync(contract, nextMonday, cancellationToken);
                    contract.LastWeeklyReminderWeekStart = nextMonday;
                    contract.UpdatedAt = VietnamTime.Now;
                    await contractRepository.UpdateAsync(contract);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Weekly reminder failed for contract {ContractId}.", contract.Id);
                }
            }
        }
        else if (day == DayOfWeek.Friday)
        {
            var filled = await weeklyJob.AutoFillNextWeekAsync(cancellationToken);
            if (filled > 0)
                _logger.LogInformation("Auto-filled weekly meals for {Count} period contracts.", filled);
        }
    }
}
