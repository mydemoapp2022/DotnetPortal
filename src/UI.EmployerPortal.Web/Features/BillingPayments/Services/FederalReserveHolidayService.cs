namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// Computes Federal Reserve holidays using official observation rules.
/// Covers: New Year's Day, MLK Day, Presidents' Day, Memorial Day, Juneteenth,
/// Independence Day, Labor Day, Columbus Day, Veterans Day, Thanksgiving, Christmas Day.
/// Weekend observed dates follow the Fed's Friday/Monday rule.
/// </summary>
public sealed class FederalReserveHolidayService : IFederalReserveHolidayService
{
    public bool IsFederalReserveHoliday(DateOnly date)
    {
        return GetHolidaysForYear(date.Year).Contains(date);
    }

    public IReadOnlyList<DateOnly> GetInvalidDatesInRange(DateOnly start, DateOnly end)
    {
        var result = new List<DateOnly>();
        var years = Enumerable.Range(start.Year, end.Year - start.Year + 1);
        var holidays = years.SelectMany(GetHolidaysForYear).ToHashSet();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || holidays.Contains(d))
                result.Add(d);
        }

        return result;
    }

    private static HashSet<DateOnly> GetHolidaysForYear(int year)
    {
        var holidays = new List<DateOnly>
        {
            Observe(new DateOnly(year, 1, 1)),                           // New Year's Day
            NthWeekdayOfMonth(year, 1, DayOfWeek.Monday, 3),            // MLK Day
            NthWeekdayOfMonth(year, 2, DayOfWeek.Monday, 3),            // Presidents' Day
            LastWeekdayOfMonth(year, 5, DayOfWeek.Monday),              // Memorial Day
            Observe(new DateOnly(year, 6, 19)),                          // Juneteenth
            Observe(new DateOnly(year, 7, 4)),                           // Independence Day
            NthWeekdayOfMonth(year, 9, DayOfWeek.Monday, 1),            // Labor Day
            NthWeekdayOfMonth(year, 10, DayOfWeek.Monday, 2),           // Columbus Day
            Observe(new DateOnly(year, 11, 11)),                         // Veterans Day
            NthWeekdayOfMonth(year, 11, DayOfWeek.Thursday, 4),         // Thanksgiving
            Observe(new DateOnly(year, 12, 25)),                         // Christmas Day
        };

        return [.. holidays];
    }

    /// <summary>Applies Fed observation rule: Saturday → Friday, Sunday → Monday.</summary>
    private static DateOnly Observe(DateOnly date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => date.AddDays(-1),
            DayOfWeek.Sunday => date.AddDays(1),
            _ => date
        };
    }

    private static DateOnly NthWeekdayOfMonth(int year, int month, DayOfWeek weekday, int nth)
    {
        var first = new DateOnly(year, month, 1);
        var offset = ((int) weekday - (int) first.DayOfWeek + 7) % 7;
        return first.AddDays(offset + (nth - 1) * 7);
    }

    private static DateOnly LastWeekdayOfMonth(int year, int month, DayOfWeek weekday)
    {
        var last = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
        var offset = ((int) last.DayOfWeek - (int) weekday + 7) % 7;
        return last.AddDays(-offset);
    }
}
