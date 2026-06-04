using UI.EmployerPortal.Generated.ServiceClients.EFTPaymentService;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// IEFTPaymentService
/// </summary>
public interface IEftPaymentService
{
    /// <summary>
    /// GetEftPaymentDatesAsync
    /// </summary>
    /// <returns></returns>
    Task<EftPaymentDatesResult> GetEftPaymentDatesAsync();

    /// <summary>
    /// Cancel Payment
    /// </summary>
    /// <param name="eftPaymentSK"></param>
    /// <param name="secureUserSK"></param>
    /// <param name="employerSK"></param>
    /// <returns></returns>
    Task<CancelEFTPaymentResponse> CancelEftPaymentAsync(int eftPaymentSK, int secureUserSK, int employerSK);

    /// <summary>
    /// Save Payment
    /// </summary>
    /// <param name="eFTPaymentRequest"></param>
    Task<SaveEFTPaymentResponse> SaveEftPaymentAsync(SaveEFTPaymentRequest eFTPaymentRequest);
}

/// <summary>EftPaymentService</summary>
public sealed class EftPaymentService : IEftPaymentService
{
    private readonly IEFTPaymentService _wcfclient;
    /// <summary>
    /// EftPaymentService
    /// </summary>
    /// <param name="wcfclient"></param>
    public EftPaymentService(IEFTPaymentService wcfclient)
    {
        _wcfclient = wcfclient;
    }
    /// <summary>
    /// GetEftPaymentDatesAsync
    /// </summary>
    /// <returns></returns>
    public async Task<EftPaymentDatesResult> GetEftPaymentDatesAsync()
    {
        var response = await _wcfclient.GetEFTPaymentDatesAsync();
        var holidays = (response.BankHolidays ?? [])
                        .Select(DateOnly.FromDateTime)
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

    /// <summary>
    /// Cancel Payment
    /// </summary>
    /// <param name="eftPaymentSK"></param>
    /// <param name="secureUserSK"></param>
    /// <param name="employerSK"></param>
    /// <returns></returns>
    public async Task<CancelEFTPaymentResponse> CancelEftPaymentAsync(int eftPaymentSK, int secureUserSK, int employerSK)
    {
        return await _wcfclient.CancelEFTPaymentAsync(eftPaymentSK, secureUserSK, employerSK);
    }

    /// <summary>
    /// Save Payment
    /// </summary>
    /// <param name="eFTPaymentRequest"></param>
    /// <returns></returns>
    public async Task<SaveEFTPaymentResponse> SaveEftPaymentAsync(SaveEFTPaymentRequest eFTPaymentRequest)
    {
        return await _wcfclient.SaveEFTPaymentAsync(eFTPaymentRequest);
    }
}
