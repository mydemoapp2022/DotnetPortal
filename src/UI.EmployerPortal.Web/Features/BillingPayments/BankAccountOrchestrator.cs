using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Services;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Session.Managers;

namespace UI.EmployerPortal.Web.Features.BillingPayments;

/// <summary>
/// Orchestrates bank account operations including routing number lookup,
/// duplicate validation, and save via the EFT payment service.
/// </summary>
public interface IBankAccountOrchestrator
{
    /// <summary>
    /// Looks up the bank name for the given 9-digit routing number.
    /// Returns null if the routing number is invalid or the service is unavailable.
    /// </summary>
    Task<string?> LookupBankNameAsync(string routingNumber);

    /// <summary>
    /// Validates duplicate rules and saves the bank account via the EFT payment service.
    /// </summary>
    Task<SaveBankAccountResult> AddBankAccountAsync(BankAccountModel model);

    /// <summary>
    /// Inactivates the selected bank account
    /// </summary>
    Task<SaveBankAccountResult> InactivateBankAccountAsync(int bankAccountSk);

    /// <summary>
    /// Returns all existing bank accounts for the currently selected employer.
    /// </summary>
    Task<IReadOnlyList<SavedBankAccount>> GetExistingAccountsAsync();
    /// <summary>
    /// GetPendingReimbursePaymentToSessionAsync
    /// </summary>
    /// <returns></returns>
    Task<ReimbursableBillingDetail?> GetPendingReimbursePaymentToSessionAsync();
    /// <summary>
    /// Stores the selected payment in session storage.
    /// </summary>
    Task SavePendingReimbursePaymentToSessionAsync(ReimbursableBillingDetail model);
}

/// <summary>
/// Default implementation of <see cref="IBankAccountOrchestrator"/>.
/// </summary>
internal class BankAccountOrchestrator : IBankAccountOrchestrator
{
    private readonly IBankAccountService _bankAccountService;
    private readonly ISessionManager _sessionManager;

    /// <summary>
    /// Initializes a new instance of <see cref="BankAccountOrchestrator"/>.
    /// </summary>
    public BankAccountOrchestrator(IBankAccountService bankAccountService, ISessionManager sessionManager)
    {
        _bankAccountService = bankAccountService;
        _sessionManager = sessionManager;
    }

    /// <inheritdoc/>
    public async Task<string?> LookupBankNameAsync(string routingNumber)
    {
        return string.IsNullOrWhiteSpace(routingNumber) || routingNumber.Length != 9
            ? null
            : await _bankAccountService.CheckRoutingNumberAsync(routingNumber);
    }

    /// <inheritdoc/>
    public async Task<SaveBankAccountResult> AddBankAccountAsync(BankAccountModel model)
    {
        var employerSk = await GetEmployerSkAsync();
        if (employerSk is null)
        {
            return new SaveBankAccountResult(false, "No employer account selected");
        }

        var existing = await _bankAccountService.GetExistingAccountsAsync(employerSk.Value);

        return existing.Any(a =>
        { return string.Equals(a.Nickname, model.Nickname, StringComparison.OrdinalIgnoreCase); })
            ? new SaveBankAccountResult(false, "An account with this nickname already exists")
            : existing.Any(a =>
            {
                return a.RoutingNumber == model.RoutingNumber &&
                                       a.MaskedAccountNumber.EndsWith(model.AccountNumber?[^4..] ?? string.Empty);
            })
            ? new SaveBankAccountResult(false, "An account with this account number already exists")
            : await _bankAccountService.SaveBankAccountAsync(model, employerSk.Value);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SavedBankAccount>> GetExistingAccountsAsync()
    {
        var employerSk = await GetEmployerSkAsync();
        return employerSk is null ? [] : await _bankAccountService.GetExistingAccountsAsync(employerSk.Value);
    }

    private async Task<int?> GetEmployerSkAsync()
    {
        var selected = await _sessionManager.GetAsync<SelectedEmployerAccount>();
        return selected?.EmployerAccount?.Id;
    }

    /// <inheritdoc/>
    public async Task<SaveBankAccountResult> InactivateBankAccountAsync(int bankAccountSk)
    {
        var employerSk = await GetEmployerSkAsync();

        return employerSk is null ? new SaveBankAccountResult(false, "No employer account selected") : await _bankAccountService.InactivateBankAccountAsync(bankAccountSk, employerSk.Value);
    }

    public async Task SavePendingReimbursePaymentToSessionAsync(ReimbursableBillingDetail model)
    {
        var selectedEmployer = await _sessionManager.GetAsync<SelectedEmployerAccount>();
        if (selectedEmployer != null)
        {
            selectedEmployer.SelectPayment = model;
            await _sessionManager.SetAsync(selectedEmployer);
        }
    }
    public async Task<ReimbursableBillingDetail?> GetPendingReimbursePaymentToSessionAsync()
    {
        var selectedEmployer = await _sessionManager.GetAsync<SelectedEmployerAccount>();
        return selectedEmployer?.SelectPayment;
    }
}
