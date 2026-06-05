namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// Abstraction for the third-party card payment provider integration.
/// Implementation details (provider name, payload, response) are pending
/// business confirmation — see open questions in Jira ticket.
/// </summary>
public interface ICardPaymentService
{
    /// <summary>
    /// Initiates a card payment session with the third-party provider.
    /// Returns a session token / launch URL used to open the provider modal.
    /// </summary>
    /// <param name="amount">Payment amount in dollars.</param>
    /// <param name="contactName">Full name of the payer.</param>
    /// <param name="email">Payer email address.</param>
    /// <param name="addressLine1">Billing address line 1.</param>
    /// <param name="city">Billing city.</param>
    /// <param name="state">Billing state (US only).</param>
    /// <param name="zip">Billing zip code.</param>
    /// <returns>
    /// A <see cref="CardPaymentSessionResult"/> with a session token or launch URL,
    /// or an error message when initiation fails.
    /// </returns>
    Task<CardPaymentSessionResult> CreateSessionAsync(
        decimal amount,
        string contactName,
        string email,
        string addressLine1,
        string city,
        string? state,
        string zip);
}

/// <summary>Result returned by <see cref="ICardPaymentService.CreateSessionAsync"/>.</summary>
public sealed record CardPaymentSessionResult
{
    /// <summary>True when the provider accepted the session request.</summary>
    public bool Success { get; init; }

    /// <summary>
    /// Token or URL passed to the provider modal (JS interop).
    /// Populated only when <see cref="Success"/> is true.
    /// </summary>
    public string? SessionToken { get; init; }

    /// <summary>Error description when <see cref="Success"/> is false.</summary>
    public string? ErrorMessage { get; init; }
}
