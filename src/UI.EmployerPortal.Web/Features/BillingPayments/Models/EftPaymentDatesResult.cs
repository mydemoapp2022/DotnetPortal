//using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>
/// Mapped result from the WCF GetEFTPaymentDates response.
/// </summary>
public sealed class EftPaymentDatesResult
{
    /// <summary>BankHolidays</summary>
    public IReadOnlyList<DateOnly> BankHolidays { get; init; } = [];
    /// <summary>FirstAvailableSettlementDate</summary>
    public DateOnly FirstAvailableSettlementDate { get; init; }
}
