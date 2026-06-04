using UI.EmployerPortal.Web.Features.BillingPayments.Models;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Services;

/// <summary>
/// Wraps EFT payment WCF service calls for bank account operations.
/// </summary>
internal interface IBankAccountService
{
    /// <summary>
    /// Calls the routing number lookup and returns the bank name.
    /// Returns null if the number is invalid or the service is unavailable.
    /// </summary>
    Task<string?> CheckRoutingNumberAsync(string routingNumber);

    /// <summary>
    /// Submits the bank account to the EFT payment service.
    /// Returns a result indicating success or the first rule violation message.
    /// </summary>
    Task<SaveBankAccountResult> SaveBankAccountAsync(BankAccountModel model, int employerAccountSk);

    /// <summary>
    /// Returns all active bank accounts on record for the given employer.
    /// Returns an empty list if none exist or the service is unavailable.
    /// </summary>
    Task<IReadOnlyList<SavedBankAccount>> GetExistingAccountsAsync(int employerAccountSk);

    /// <summary>
    /// Inactivates the specified bank account for the currently selected employer
    /// Returns a result indicating success or the first rule violation message.
    /// </summary>
    Task<SaveBankAccountResult> InactivateBankAccountAsync(int bankAccountSk, int employerAccountSk);
}

/// <summary>
/// Represents the outcome of a save bank account operation.
/// </summary>
/// <param name="Success">True if the account was saved without rule violations.</param>
/// <param name="ErrorMessage">The first rule violation message when <paramref name="Success"/> is false.</param>
public sealed record SaveBankAccountResult(bool Success, string? ErrorMessage = null);
