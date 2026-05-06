namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// Provides Federal Reserve holiday data for settlement date validation.
/// </summary>
public interface IFederalReserveHolidayService
{
    /// <summary>
    /// Returns whether the given date is a Federal Reserve holiday.
    /// </summary>
    bool IsFederalReserveHoliday(DateOnly date);

    /// <summary>
    /// Returns all invalid settlement dates (weekends + Fed holidays) in [start, end].
    /// </summary>
    IReadOnlyList<DateOnly> GetInvalidDatesInRange(DateOnly start, DateOnly end);
}
