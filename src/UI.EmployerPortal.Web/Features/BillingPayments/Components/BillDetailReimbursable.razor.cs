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
    [Inject]
    private IUserAccountService UserAccountService { get; set; } = default!;
    [Inject]
    private IDashboardOrchestrator DashboardOrchestrator { get; set; } = default!;

    [Inject]
    private IBankAccountOrchestrator BankAccountOrchestrator { get; set; } = default!;
    /// <summary>
    /// 
    /// </summary>
    public ReimbursableBillingDetail Model { get; set; } = new();

    [Inject]
    private IBillDetailServices BillDetailServices { get; set; } = default!;
    private EmployerAccount? _employerSK;
    private EditContext? _editContext;
    private readonly HashSet<FieldIdentifier> _touchedFields = new();
    private string? _selectedpayment;

    /// <summary>
    /// OnInitializedAsync
    /// </summary>
    /// <returns></returns>
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
        _editContext.OnFieldChanged += (_, e) =>
        {
            _editContext.Validate();
            StateHasChanged();
        };
        
        await base.OnInitializedAsync();
    }
    private async Task PaymentAsync()
    {
        //Nav.NavigateTo(Nav.BaseUri);
        if (_selectedpayment == "ACH")
        {
            Model.SelectedPaymentMethod = _selectedpayment;
            await BankAccountOrchestrator.SavePendingReimbursePaymentToSessionAsync(Model);
            Nav.NavigateTo("billing-payments/bank-account-payment-ach");
        }

    }

    private void OnpaymentChanged(ChangeEventArgs e)
    {
        _selectedpayment = e.Value?.ToString() ?? "";

    }

    private void HandleContinue()
    {
        StateHasChanged();
    }

    /// <summary>Called by EditForm when top-level validation fails.</summary>

    private void OnInvalid()
    {
        StateHasChanged();
    }
    /// <summary>
    /// Gets the current validation state.
    /// </summary>
    public bool IsValid => _editContext?.Validate() ?? false;
    /// <summary>
    /// Validates the form and returns true if valid, false otherwise.
    /// Called by parent wizard to validate before navigation.
    /// </summary>
    public bool Validate()
    {
        if (_editContext == null)
        {
            return false;
        }


        var isValid = _editContext.Validate();
        StateHasChanged();
        return isValid;
    }
}
