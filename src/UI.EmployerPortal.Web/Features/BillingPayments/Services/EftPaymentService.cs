using UI.EmployerPortal.Web.Features.BillingPayments.Models;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// Abstraction over the WCF EFT Payment Service.
/// </summary>
public interface IEftPaymentService
{
    /// <summary>
    /// Gets the EFT payment dates.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the EFT payment dates.</returns>
    Task<EftPaymentDatesResult> GetEftPaymentDatesAsync();
}

/// <summary>
/// Calls the WCF GetEFTPaymentDates service and maps the response.
/// </summary>
public sealed class EftPaymentService : IEftPaymentService
{
    private readonly IEFTPaymentService _wcfClient;   // ← your WCF proxy interface

    public EftPaymentService(IEFTPaymentService wcfClient)
    {
        _wcfClient = wcfClient;
    }

    public async Task<EftPaymentDatesResult> GetEftPaymentDatesAsync()
    {
        var response = await _wcfClient.GetEFTPaymentDatesAsync();

        var holidays = (response.BankHolidays ?? [])
            .Select(dt => DateOnly.FromDateTime(dt))
            .ToList();

        var firstAvailable = response.FirstAvailableSettlementDate?.Value is { } dt
            ? DateOnly.FromDateTime(dt)
            : DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        return new EftPaymentDatesResult
        {
            BankHolidays = holidays,
            FirstAvailableSettlementDate = firstAvailable
        };
    }
}
