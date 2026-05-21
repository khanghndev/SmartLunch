using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Services;

public static class BackupScheduleCalculator
{
    public static DateTime? ComputeNextRun(SystemBackupSchedule schedule)
    {
        if (!schedule.IsEnabled)
            return null;

        var now = VietnamTime.Now;
        var mode = (schedule.ScheduleMode ?? "Daily").Trim();

        return mode switch
        {
            "Once" => ComputeOnce(schedule, now),
            "Weekly" => ComputeWeekly(schedule, now),
            _ => ComputeDaily(schedule, now)
        };
    }

    private static DateTime? ComputeOnce(SystemBackupSchedule schedule, DateTime now)
    {
        if (!schedule.OnceScheduledAt.HasValue)
            return null;

        var target = schedule.OnceScheduledAt.Value;
        return target > now ? target : null;
    }

    private static DateTime ComputeDaily(SystemBackupSchedule schedule, DateTime now)
    {
        var time = MinutesToTimeOfDay(schedule.TimeOfDayMinutes);
        var candidate = now.Date.Add(time);
        if (candidate <= now)
            candidate = candidate.AddDays(1);
        return candidate;
    }

    private static DateTime? ComputeWeekly(SystemBackupSchedule schedule, DateTime now)
    {
        if (!schedule.DayOfWeek.HasValue)
            return ComputeDaily(schedule, now);

        var targetDow = schedule.DayOfWeek.Value;
        var time = MinutesToTimeOfDay(schedule.TimeOfDayMinutes);

        for (var i = 0; i < 8; i++)
        {
            var day = now.Date.AddDays(i);
            if ((int)day.DayOfWeek != targetDow)
                continue;

            var candidate = day.Add(time);
            if (candidate > now)
                return candidate;
        }

        return now.Date.AddDays(7).Add(time);
    }

    public static int ParseTimeOfDayMinutes(string? hhmm)
    {
        if (string.IsNullOrWhiteSpace(hhmm))
            return 21 * 60; // 21:00 mặc định

        var parts = hhmm.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m))
            return 180;

        h = Math.Clamp(h, 0, 23);
        m = Math.Clamp(m, 0, 59);
        return h * 60 + m;
    }

    public static string FormatTimeOfDayMinutes(int minutes)
    {
        minutes = Math.Clamp(minutes, 0, 1439);
        return $"{minutes / 60:D2}:{minutes % 60:D2}";
    }

    private static TimeSpan MinutesToTimeOfDay(int minutes)
    {
        minutes = Math.Clamp(minutes, 0, 1439);
        return new TimeSpan(minutes / 60, minutes % 60, 0);
    }
}
