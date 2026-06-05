using System.ServiceModel;
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

    /// <summary>
    /// Returns all US state codes from the WCF <c>GetUSStateCodes</c> operation.
    /// Each item contains <c>CodeSK</c>, <c>LongDescription</c> (full state name),
    /// and <c>ShortDescription</c> (two-letter abbreviation, e.g. "WI").
    /// </summary>
    Task<IReadOnlyList<CodeLookupItem>> GetUSStateCodesAsync();

    /// <summary>
    /// Returns all country codes from the WCF <c>GetCountryCodes</c> operation.
    /// Each item contains <c>CodeSK</c>, <c>LongDescription</c> (country name),
    /// and <c>ShortDescription</c> (two-letter ISO code, e.g. "US").
    /// </summary>
    Task<IReadOnlyList<CodeLookupItem>> GetCountryCodesAsync();
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

    /// <summary>
    /// Get US state codes from the WCF <c>GetUSStateCodes</c> operation.
    /// Each item contains <c>CodeSK</c>, <c>LongDescription</c> (full state name),
    /// and <c>ShortDescription</c> (two-letter abbreviation, e.g. "WI").
    /// </summary>
    public async Task<IReadOnlyList<CodeLookupItem>> GetUSStateCodesAsync()
    {
        try
        {
            var items = await _wcfclient.GetUSStateCodesAsync();

            if (items is null || items.Length == 0)
                return [];

            return items
                .Select(s => new CodeLookupItem
                {
                    CodeSK = s.CodeSK,
                    LongDescription = s.LongDescription ?? string.Empty,
                    ShortDescription = s.ShortDescription ?? string.Empty
                })
                .OrderBy(s => s.LongDescription)
                .ToList();
        }
        catch (CommunicationException)
        {
            return [];
        }
    }

    /// <summary>
    /// Get country codes from the WCF <c>GetCountryCodes</c> operation.
    /// Each item contains <c>CodeSK</c>, <c>LongDescription</c> (country name),
    /// and <c>ShortDescription</c> (two-letter ISO code, e.g. "US").
    /// </summary>
    public async Task<IReadOnlyList<CodeLookupItem>> GetCountryCodesAsync()
    {
        try
        {
            var items = await _wcfclient.GetCountryCodesAsync();

            if (items is null || items.Length == 0)
                return [];

            return items
                .Select(c => new CodeLookupItem
                {
                    CodeSK = c.CodeSK,
                    LongDescription = c.LongDescription ?? string.Empty,
                    ShortDescription = c.ShortDescription ?? string.Empty
                })
                .OrderBy(c => c.LongDescription)
                .ToList();
        }
        catch (CommunicationException)
        {
            return [];
        }
    }
}
