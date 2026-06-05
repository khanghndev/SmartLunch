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
/// Mỗi ngày 18h–20h (VN): nhắc đặt món trước 3 ngày tuần mở; auto random + email nếu quá hạn (Chủ nhật trước tuần phục vụ).
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

        using var scope = _scopeFactory.CreateScope();
        var contractRepository = scope.ServiceProvider.GetRequiredService<IContractRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IOrganizationOrderEmailService>();
        var weeklyJob = scope.ServiceProvider.GetRequiredService<OrganizationMealContractWeeklyJobService>();
        var weeklySelectionService = scope.ServiceProvider.GetRequiredService<OrganizationMealContractWeeklySelectionService>();

        var today = DateOnly.FromDateTime(vnNow);
        var contracts = await contractRepository.GetActivePeriodBasedContractsAsync(cancellationToken);

        foreach (var contract in contracts)
        {
            if (!contract.EndDate.HasValue)
                continue;

            var contractStart = DateOnly.FromDateTime(contract.StartDate);
            var contractEnd = DateOnly.FromDateTime(contract.EndDate.Value);
            var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToList();
            var openWeekMonday = OrganizationMealWeeklySelectionRules.ResolveOpenWeekMonday(
                today, contractStart, contractEnd, excluded);
            if (!openWeekMonday.HasValue)
                continue;

            var reminderDay = openWeekMonday.Value.AddDays(-3);
            if (today == reminderDay
                && contract.LastWeeklyReminderWeekStart != openWeekMonday.Value
                && !await weeklySelectionService.WeekIsFilledAsync(contract.Id, openWeekMonday.Value, cancellationToken))
            {
                try
                {
                    await emailService.SendWeeklyMealSelectionReminderAsync(
                        contract, openWeekMonday.Value, cancellationToken);
                    contract.LastWeeklyReminderWeekStart = openWeekMonday.Value;
                    contract.UpdatedAt = VietnamTime.Now;
                    await contractRepository.UpdateAsync(contract);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Weekly reminder failed for contract {ContractId}.", contract.Id);
                }
            }

            var autoFillDay = openWeekMonday.Value.AddDays(-1);
            if (today == autoFillDay
                && contract.WeeklyAutoFillWeekStart != openWeekMonday.Value
                && !await weeklySelectionService.WeekIsFilledAsync(contract.Id, openWeekMonday.Value, cancellationToken))
            {
                try
                {
                    var result = await weeklyJob.TryAutoFillOpenWeekForContractAsync(contract, cancellationToken);
                    if (result.HasValue)
                    {
                        var (filledContract, weekMonday) = result.Value;
                        filledContract.WeeklyAutoFillWeekStart = weekMonday;
                        filledContract.UpdatedAt = VietnamTime.Now;
                        await contractRepository.UpdateAsync(filledContract);
                        await emailService.SendWeeklyMealAutoFilledAsync(
                            filledContract, weekMonday, cancellationToken);
                        _logger.LogInformation(
                            "Auto-filled weekly meals for contract {ContractId}, week {WeekStart}.",
                            filledContract.Id,
                            weekMonday);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Weekly auto-fill failed for contract {ContractId}.", contract.Id);
                }
            }
        }
    }
}
