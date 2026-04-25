// using Microsoft.EntityFrameworkCore;
// using SmartLunch.Backend.Service.Application.Constants;
// using SmartLunch.Backend.Service.Infrastructure.Data;
// using SmartLunch.Backend.Service.Domain.Entities;

// namespace SmartLunch.Backend.Service.API.Services.Jobs;

// public class AutoLockOrdersService : BackgroundService
// {
//     private readonly IServiceProvider _services;
//     private readonly ILogger<AutoLockOrdersService> _logger;

//     public AutoLockOrdersService(IServiceProvider services, ILogger<AutoLockOrdersService> logger)
//     {
//         _services = services;
//         _logger = logger;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         _logger.LogInformation("AutoLockOrdersService is starting.");
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 await ProcessAutoLockOrders(stoppingToken);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error occurred during auto-locking orders");
//             }

//             // Run every 30 minutes
//             await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
//         }
//     }

//     private async Task ProcessAutoLockOrders(CancellationToken stoppingToken)
//     {
//         using var scope = _services.CreateScope();
//         var context = scope.ServiceProvider.GetRequiredService<SmartLunchDBContext>();

//         var nowUtc = DateTime.UtcNow;
//         var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
//         var nowVn = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, vnTimeZone);

//         // Fetch pending/confirmed orders that haven't been locked to "preparing" or passed that
//         var openOrders = await context.Orders
//             .Include(o => o.Unit)
//             .Where(o => o.Status == OrderLifecycleStatus.Pending || o.Status == OrderLifecycleStatus.Confirmed)
//             .ToListAsync(stoppingToken);

//         int lockedCount = 0;

//         foreach (var order in openOrders)
//         {
//             var unitType = order.Unit?.UnitType ?? "Office";
//             var scheduledVn = order.ScheduledDate.Kind == DateTimeKind.Utc 
//                 ? TimeZoneInfo.ConvertTimeFromUtc(order.ScheduledDate, vnTimeZone) 
//                 : order.ScheduledDate;
            
//             var daysDifference = (scheduledVn.Date - nowVn.Date).Days;

//             bool shouldLock = false;

//             if (unitType.Equals("Office", StringComparison.OrdinalIgnoreCase))
//             {
//                 if (daysDifference < 1 || (daysDifference == 1 && nowVn.Hour >= 17))
//                     shouldLock = true;
//             }
//             else if (unitType.Equals("Factory", StringComparison.OrdinalIgnoreCase))
//             {
//                 if (daysDifference < 2)
//                     shouldLock = true;
//             }
//             else if (unitType.Equals("School", StringComparison.OrdinalIgnoreCase))
//             {
//                 if (daysDifference < 3)
//                     shouldLock = true;
//             }

//             if (shouldLock)
//             {
//                 order.Status = OrderLifecycleStatus.Preparing; // Move to locked state for production
//                 lockedCount++;
//             }
//         }

//         if (lockedCount > 0)
//         {
//             await context.SaveChangesAsync(stoppingToken);
//             _logger.LogInformation("Successfully locked and moved {Count} orders to 'preparing' status as they reached their cut-off times.", lockedCount);
//         }
//     }
// }
