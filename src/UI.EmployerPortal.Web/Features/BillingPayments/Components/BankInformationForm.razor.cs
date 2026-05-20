using Microsoft.AspNetCore.Components;
using UI.EmployerPortal.Razor.SharedComponents.Inputs;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Components;

/// <summary>
/// Code-behind for the Bank Information.
/// </summary>
public partial class BankInformationForm
{
    [Inject] private IBankAccountOrchestrator BankAccountOrchestrator { get; set; } = default!;
    /// <summary>
    /// On Save
    /// </summary>
    [Parameter] public EventCallback<SavedBankAccount> OnSaved { get; set; }
    /// <summary>
    /// On Back 
    /// </summary>
    [Parameter] public EventCallback OnBack { get; set; }
    /// <summary>
    /// On Cancel
    /// </summary>
    [Parameter] public EventCallback OnCancel { get; set; }

    private readonly BankAccountModel _model = new();
    private bool _showValidation;
    private bool _isSaving;
    private bool _showIatExplanation;
    private string? _saveError;

    private readonly List<SelectOption> _accountTypeOptions =
    [
        new SelectOption { Value = "Checking", Text = "Checking" },
        new SelectOption { Value = "Savings",  Text = "Savings"  }
    ];

    private readonly List<SelectOption> _countryOptions = BuildCountryOptions();

    private async Task HandleRoutingBlur()
    {
        if (_model.RoutingNumber?.Length == 9)
        {
            _model.BankName = await BankAccountOrchestrator.LookupBankNameAsync(_model.RoutingNumber);
        }
    }

    private void HandleIatChanged(bool value)
    {
        _model.IsInternational = value;

        if (!value)
        {
            _model.IatCountryCode = 0;
            _model.IatStreetAddress = null;
            _model.IatCity = null;
            _model.IatPostalCode = null;
        }
    }

    private void HandleCountryChanged(string value)
    {
        _model.IatCountryCode = int.TryParse(value, out var code) ? code : 0;
    }

    private void ToggleIatExplanation()
    {
        _showIatExplanation = !_showIatExplanation;
    }

    private async Task HandleValidSubmit()
    {
        await SaveAsync();
    }

    private async Task HandleSaveAndContinue()
    {
        _showValidation = true;
        await SaveAsync();
    }

    private async Task SaveAsync()
    {
        _isSaving = true;
        _saveError = null;

        var result = await BankAccountOrchestrator.AddBankAccountAsync(_model);

        _isSaving = false;

        if (!result.Success)
        {
            _saveError = result.ErrorMessage;
            return;
        }

        var saved = new SavedBankAccount
        {
            Nickname = _model.Nickname ?? string.Empty,
            RoutingNumber = _model.RoutingNumber ?? string.Empty,
            MaskedAccountNumber = MaskAccountNumber(_model.AccountNumber),
            BankName = _model.BankName ?? string.Empty,
            AccountType = _model.AccountType ?? string.Empty
        };

        await OnSaved.InvokeAsync(saved);
    }

    private void HandleBack()
    {
        _ = OnBack.InvokeAsync();
    }

    private static string MaskAccountNumber(string? accountNumber)
    {
        return string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 4
            ? accountNumber ?? string.Empty
            : $"*******{accountNumber[^4..]}";
    }

    private static List<SelectOption> BuildCountryOptions()
    {
        return [
            new SelectOption { Value = "124", Text = "Canada" },
        new SelectOption { Value = "484", Text = "Mexico" },
        new SelectOption { Value = "826", Text = "United Kingdom" },
        new SelectOption { Value = "276", Text = "Germany" },
        new SelectOption { Value = "250", Text = "France" },
        new SelectOption { Value = "392", Text = "Japan" },
        new SelectOption { Value = "36",  Text = "Australia" },
        new SelectOption { Value = "999", Text = "Other" }
        ];
    }
}
