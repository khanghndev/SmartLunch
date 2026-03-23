using System.Globalization;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetCashflowSummary;

public class GetCashflowSummaryQueryHandler : IRequestHandler<GetCashflowSummaryQuery, GetCashflowSummaryResponse>
{
    private const int MaxRangeDays = 800;

    private readonly IFinanceAnalyticsRepository _financeRepository;
    private readonly ILogger<GetCashflowSummaryQueryHandler> _logger;

    public GetCashflowSummaryQueryHandler(
        IFinanceAnalyticsRepository financeRepository,
        ILogger<GetCashflowSummaryQueryHandler> logger)
    {
        _financeRepository = financeRepository;
        _logger = logger;
    }

    public async Task<GetCashflowSummaryResponse> Handle(GetCashflowSummaryQuery request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.From == default || req.To == default)
            throw new ArgumentException("From and To are required.");
        if (req.To < req.From)
            throw new ArgumentException("'To' must be on or after 'From'.");

        var spanDays = req.To.DayNumber - req.From.DayNumber + 1;
        if (spanDays > MaxRangeDays)
            throw new ArgumentException($"Date range cannot exceed {MaxRangeDays} days.");

        var rangeStart = req.From.ToDateTime(TimeOnly.MinValue);
        var rangeEndExclusive = req.To.ToDateTime(TimeOnly.MinValue).AddDays(1);

        var payments = await _financeRepository.GetPaidPaymentsInRangeAsync(rangeStart, rangeEndExclusive, cancellationToken);
        var transactions = await _financeRepository.GetTransactionsInRangeAsync(rangeStart, rangeEndExclusive, cancellationToken);

        var orderedKeys = EnumeratePeriodKeys(req.From, req.To, req.Granularity).ToList();
        var collected = orderedKeys.ToDictionary(k => k, _ => 0m);
        var income = orderedKeys.ToDictionary(k => k, _ => 0m);
        var expense = orderedKeys.ToDictionary(k => k, _ => 0m);

        foreach (var p in payments)
        {
            var key = GetPeriodKey(p.PaymentDate, req.Granularity);
            if (collected.ContainsKey(key))
                collected[key] += p.Amount;
        }

        foreach (var t in transactions)
        {
            var key = GetPeriodKey(t.Date, req.Granularity);
            if (!income.ContainsKey(key))
                continue;
            if (t.Amount > 0)
                income[key] += t.Amount;
            else if (t.Amount < 0)
                expense[key] += -t.Amount;
        }

        var buckets = new List<CashflowBucketDto>();
        decimal totalPay = 0, totalInc = 0, totalExp = 0;

        foreach (var key in orderedKeys)
        {
            var c = collected[key];
            var i = income[key];
            var e = expense[key];
            var net = c + i - e;
            totalPay += c;
            totalInc += i;
            totalExp += e;

            var (start, end) = GetPeriodBounds(key, req.Granularity);
            buckets.Add(new CashflowBucketDto
            {
                PeriodKey = key,
                PeriodStart = start,
                PeriodEnd = end,
                CollectedPayments = c,
                TransactionIncome = i,
                TransactionExpense = e,
                NetFlow = net
            });
        }

        _logger.LogInformation(
            "Cashflow summary {From}–{To} granularity {Granularity}: {BucketCount} buckets",
            req.From, req.To, req.Granularity, buckets.Count);

        return new GetCashflowSummaryResponse
        {
            From = req.From,
            To = req.To,
            Granularity = req.Granularity.ToString(),
            TotalCollectedPayments = totalPay,
            TotalTransactionIncome = totalInc,
            TotalTransactionExpense = totalExp,
            TotalNet = totalPay + totalInc - totalExp,
            Buckets = buckets
        };
    }

    private static IEnumerable<string> EnumeratePeriodKeys(DateOnly from, DateOnly to, CashflowGranularity g)
    {
        return g switch
        {
            CashflowGranularity.Day => EnumerateDayKeys(from, to),
            CashflowGranularity.Month => EnumerateMonthKeys(from, to),
            CashflowGranularity.Week => EnumerateWeekKeys(from, to),
            _ => EnumerateDayKeys(from, to)
        };
    }

    private static IEnumerable<string> EnumerateDayKeys(DateOnly from, DateOnly to)
    {
        for (var d = from; d <= to; d = d.AddDays(1))
            yield return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static IEnumerable<string> EnumerateMonthKeys(DateOnly from, DateOnly to)
    {
        var cursor = new DateOnly(from.Year, from.Month, 1);
        var end = new DateOnly(to.Year, to.Month, 1);
        while (cursor <= end)
        {
            yield return cursor.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            cursor = cursor.AddMonths(1);
        }
    }

    private static IEnumerable<string> EnumerateWeekKeys(DateOnly from, DateOnly to)
    {
        var set = new HashSet<(int Year, int Week)>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            var dt = d.ToDateTime(TimeOnly.MinValue);
            set.Add((ISOWeek.GetYear(dt), ISOWeek.GetWeekOfYear(dt)));
        }

        return set
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Week)
            .Select(x => $"{x.Year}-W{x.Week:D2}");
    }

    private static string GetPeriodKey(DateTime at, CashflowGranularity g)
    {
        var d = DateOnly.FromDateTime(at);
        return g switch
        {
            CashflowGranularity.Day => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            CashflowGranularity.Month => d.ToString("yyyy-MM", CultureInfo.InvariantCulture),
            CashflowGranularity.Week => $"{ISOWeek.GetYear(at)}-W{ISOWeek.GetWeekOfYear(at):D2}",
            _ => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
    }

    private static (DateTime Start, DateTime End) GetPeriodBounds(string periodKey, CashflowGranularity g)
    {
        return g switch
        {
            CashflowGranularity.Day => GetDayBounds(periodKey),
            CashflowGranularity.Month => GetMonthBounds(periodKey),
            CashflowGranularity.Week => GetWeekBounds(periodKey),
            _ => GetDayBounds(periodKey)
        };
    }

    private static (DateTime Start, DateTime End) GetDayBounds(string periodKey)
    {
        var d = DateOnly.ParseExact(periodKey, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var start = d.ToDateTime(TimeOnly.MinValue);
        var end = d.ToDateTime(TimeOnly.MaxValue);
        return (start, end);
    }

    private static (DateTime Start, DateTime End) GetMonthBounds(string periodKey)
    {
        var parts = periodKey.Split('-', 2, StringSplitOptions.RemoveEmptyEntries);
        var year = int.Parse(parts[0], CultureInfo.InvariantCulture);
        var month = int.Parse(parts[1], CultureInfo.InvariantCulture);
        var first = new DateOnly(year, month, 1);
        var last = first.AddMonths(1).AddDays(-1);
        return (first.ToDateTime(TimeOnly.MinValue), last.ToDateTime(TimeOnly.MaxValue));
    }

    private static (DateTime Start, DateTime End) GetWeekBounds(string periodKey)
    {
        var idx = periodKey.IndexOf("-W", StringComparison.Ordinal);
        var year = int.Parse(periodKey[..idx], CultureInfo.InvariantCulture);
        var week = int.Parse(periodKey[(idx + 2)..], CultureInfo.InvariantCulture);
        var monday = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        var sunday = monday.AddDays(6);
        return (
            DateOnly.FromDateTime(monday).ToDateTime(TimeOnly.MinValue),
            DateOnly.FromDateTime(sunday).ToDateTime(TimeOnly.MaxValue));
    }
}
