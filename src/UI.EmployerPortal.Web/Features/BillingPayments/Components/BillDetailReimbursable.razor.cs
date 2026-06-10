using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.BillingPayments.Services;
using UI.EmployerPortal.Web.Features.Dashboard;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using EmployerRequest = UI.EmployerPortal.Generated.ServiceClients.BillDetailService.EmployerRequest;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Components;
/// <summary>
/// 
/// </summary>
public partial class BillDetailReimbursable
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IUserAccountService UserAccountService { get; set; } = default!;
    [Inject] private IDashboardOrchestrator DashboardOrchestrator { get; set; } = default!;
    [Inject] private IBankAccountOrchestrator BankAccountOrchestrator { get; set; } = default!;
    [Inject] private IBillDetailServices BillDetailServices { get; set; } = default!;
    public ReimbursableBillingDetail Model { get; set; } = new();
    [Inject] private IBillDetailServices BillDetailServices { get; set; } = default!;
    private EmployerAccount? _employerSK;
    private EditContext? _editContext;
    private readonly HashSet<FieldIdentifier> _touchedFields = new();
    private string? _selectedpayment;
    private bool _showValidationSummary;
    private readonly List<string> _validationErrors = [];
    private bool _amountHasError;
    private bool _paymentMethodHasError;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(Model);
    }

    /// <summary>OnInitializedAsync</summary>
    protected override async Task OnInitializedAsync()
    {
        _employerSK = await DashboardOrchestrator.GetSelectedEmployerAccountAsync();
        var secureUserSK = UserAccountService.GetUserSKClaim();
        var employerSk = _employerSK?.Id ?? 0;

        var request = new EmployerRequest
        {
            EmployerSK = employerSk,
            SecureUserSK = secureUserSK
        };

        var result = await BillDetailServices.GetReimburseBillingDetail(request);

        if (result != null)
        {
            Model.AmountToPay = result.AmountToPay;
            Model.TotalEFTPayments = result.TotalEFTPayments;
            Model.TotalOutstandingBalance = result.TotalOutstandingBalance;
        }

        var sessionData = await BankAccountOrchestrator.GetPendingReimbursePaymentToSessionAsync();
        if (sessionData != null)
        {
            if (sessionData.AmountToPay > 0)
            {
                Model.AmountToPay = sessionData.AmountToPay;
            }

            if (!string.IsNullOrEmpty(sessionData.SelectedPaymentMethod))
            {
                _selectedpayment = sessionData.SelectedPaymentMethod;
            }
        }

        _editContext = new EditContext(Model);
        _editContext.OnFieldChanged += (_, _) =>
        {
            if (_showValidationSummary)
            {
                ValidateForBanner();
            }
        };

        await base.OnInitializedAsync();
    }
    private void OnAmountChanged(decimal? value)
    {
        Model.AmountToPay = value ?? 0m;
        ValidateForBanner();
    }

    private async Task PaymentAsync()
    {
        if (!ValidateForBanner())
        {
            return;
        }

        if (_selectedpayment == "ACH")
        {
            Model.SelectedPaymentMethod = _selectedpayment;
            await BankAccountOrchestrator.SavePendingReimbursePaymentToSessionAsync(Model);
            Nav.NavigateTo("billing-payments/bank-account-payment-ach");
        }
        else if (_selectedpayment == "Card")
        {
            Model.SelectedPaymentMethod = _selectedpayment;
            await BankAccountOrchestrator.SavePendingReimbursePaymentToSessionAsync(Model);
            Nav.NavigateTo("billing-payments/card-payment");
        }

    }

    private void OnpaymentChanged(ChangeEventArgs e)
    {
        _selectedpayment = e.Value?.ToString() ?? string.Empty;
        ValidateForBanner();
    }

    private void HandleContinue()
    {
        ValidateForBanner();
        StateHasChanged();
    }

    /// <summary>Called by EditForm when top-level validation fails.</summary>

    private void OnInvalid()
    {
        ValidateForBanner();
        StateHasChanged();
    }
    /// <summary>
    /// Gets the current validation state.
    /// </summary>
    public bool IsValid => _editContext?.Validate();
    /// <summary>
    /// Validates the form and returns true if valid, false otherwise.
    /// Called by parent wizard to validate before navigation.
    /// </summary>
    public bool Validate()
    {
        var isValid = _editContext.Validate();
        var customValid = ValidateForBanner();
        StateHasChanged();
        return isValid && customValid;
    }

    private bool ValidateForBanner()
    {
        _validationErrors.Clear();

        _amountHasError = Model.AmountToPay <= 0;
        _paymentMethodHasError = string.IsNullOrWhiteSpace(_selectedpayment);

        if (_amountHasError)
        {
            _validationErrors.Add("Amount to Pay is required and must be greater than $0.00.");
        }

        if (_paymentMethodHasError)
        {
            _validationErrors.Add("Payment Method Selection is required.");
        }

        _showValidationSummary = _validationErrors.Count > 0;
        StateHasChanged();

        return !_showValidationSummary;
    }
}
