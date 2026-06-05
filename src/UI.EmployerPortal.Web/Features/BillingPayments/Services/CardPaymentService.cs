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

/// <summary>
/// Stub implementation used until the third-party card payment provider
/// integration contract is confirmed.
/// Replace this class with the real provider implementation once the
/// provider name, API endpoint, and request/response payload are confirmed.
/// </summary>
internal sealed class CardPaymentService : ICardPaymentService
{
    /// <inheritdoc/>
    public Task<CardPaymentSessionResult> CreateSessionAsync(
        decimal amount,
        string contactName,
        string email,
        string addressLine1,
        string city,
        string? state,
        string zip)
    {
        // TODO: Replace with real third-party provider call once integration
        // details are confirmed (provider name, API endpoint, payload, modal type).
        var result = new CardPaymentSessionResult
        {
            Success = true,
            SessionToken = "STUB_TOKEN",
            ErrorMessage = null
        };

        return Task.FromResult(result);
    }
}
