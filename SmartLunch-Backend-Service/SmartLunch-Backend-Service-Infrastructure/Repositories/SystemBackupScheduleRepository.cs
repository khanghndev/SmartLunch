using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class SystemBackupScheduleRepository : ISystemBackupScheduleRepository
{
    private readonly SmartLunchDBContext _context;

    public SystemBackupScheduleRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<SystemBackupSchedule> GetOrCreateAsync()
    {
        var row = await _context.SystemBackupSchedules.FirstOrDefaultAsync(e => e.Id == 1);
        if (row != null)
            return row;

        row = new SystemBackupSchedule
        {
            Id = 1,
            IsEnabled = false,
            ScheduleMode = "Daily",
            TimeOfDayMinutes = 21 * 60,
            UpdatedAt = VietnamTime.Now
        };
        _context.SystemBackupSchedules.Add(row);
        await _context.SaveChangesAsync();
        return row;
    }

    public async Task<SystemBackupSchedule> UpdateAsync(SystemBackupSchedule schedule)
    {
        _context.SystemBackupSchedules.Update(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }
}
