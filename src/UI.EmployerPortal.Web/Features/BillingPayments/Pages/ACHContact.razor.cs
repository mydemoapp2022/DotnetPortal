using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using UI.EmployerPortal.Razor.SharedComponents.Inputs;
using UI.EmployerPortal.Razor.SharedComponents.Validation;
using UI.EmployerPortal.Web.Features.BillingPayments.Models;
using UI.EmployerPortal.Web.Features.Dashboard;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Models;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using UI.EmployerPortal.Web.Features.Shared.QuarterlyTax.Services;

namespace UI.EmployerPortal.Web.Features.BillingPayments.Pages;
/// <summary>
/// ACH Contact
/// </summary>
public partial class ACHContact
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject]
    private IDashboardOrchestrator DashboardOrchestrator { get; set; } = default!;

    /// <summary>
    /// Gets or sets the primary data model for the form
    /// </summary>
    public ACHContactModel Model { get; set; } = new();
    [Inject]
    private IContactInformationService ContactInformationService { get; set; } = default!;
    [Inject]
    private IUserAccountService UserAccountService { get; set; } = default!;
    /// <summary>
    /// Tracks if the form has been attempted to be submitted
    /// </summary>
    private EditContext _editContext = default!;

    /// <summary>Reference to CustomValidator for displaying nested object errors.</summary>

    private CustomValidator? _customValidator;

    /// <summary>Tracks whether the form has been submitted at least once.</summary>

    private bool _formSubmitted = false;

    /// <summary>Tracks whether the current form state has any validation errors.</summary>

    private bool _hasValidationErrors = false;
    /// <summary>Tracks which fields have been interacted with so errors show on blur.</summary>

    private readonly HashSet<FieldIdentifier> _touchedFields = new();
    private EmployerAccount? _employerSK;
    private bool _contactexist = false;

    /// <summary>
    /// Pageload
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(Model);
        _editContext.OnFieldChanged += (_, e) =>
        {
            _touchedFields.Add(e.FieldIdentifier);
            _hasValidationErrors = _editContext.GetValidationMessages().Any();
            StateHasChanged();

        };
        //SERVICE CALL

        _employerSK = await DashboardOrchestrator.GetSelectedEmployerAccountAsync();

        var contactTypeCodeSK = 4;
        var secureUserSK = UserAccountService.GetUserSKClaim();
        var employerSk = _employerSK?.Id ?? 0;

        var result = await ContactInformationService.GetEmployerWebContact(secureUserSK, employerSk, contactTypeCodeSK);

        if (result != null)
        {
            Model = result;
            // _contactexist = true;
        }
    }
    private bool IsVisible(Expression<Func<string?>> @for)
    {
        return _formSubmitted || _touchedFields.Contains(FieldIdentifier.Create(@for));
    }
    private void HandleContinue()
    {
        _formSubmitted = true;
        _hasValidationErrors = false;
        StateHasChanged();
    }

    /// <summary>Called by EditForm when top-level validation fails.</summary>

    private void OnInvalid()
    {
        _formSubmitted = true;
        _hasValidationErrors = true;
        StateHasChanged();
    }
    /// <summary>
    /// 
    /// </summary>
    public static readonly List<SelectOption> PhoneNumberFormat = new()
    {
        new SelectOption {Value = "United States/Canada", Text = "United States/Canada"},
        new SelectOption {Value = "International", Text = "International"},

    };

    private void GoBack()
    {
        //Nav.NavigateTo(Nav.BaseUri);
        var baseUri = new Uri(Nav.BaseUri);
        var pathBase = new PathString(baseUri.AbsolutePath.TrimEnd('/'));
        Nav.NavigateTo(pathBase);
    }
    private async Task Save()
    {
        var secureUserSK = UserAccountService.GetUserSKClaim();
        var employerSk = _employerSK?.Id ?? 0;
        Model.InternationalFlag = Model.PhoneNumberFormat == "International";
        var resultsave = await ContactInformationService.SaveWebContact(Model, secureUserSK, employerSk);

        if (resultsave != null)
        {
            //test
            _contactexist = true;
            var result = await ContactInformationService.GetEmployerWebContact(UserAccountService.GetUserSKClaim(), _employerSK?.Id ?? 0, 4);
            if (result != null)
            {
                Model = result;
                // _contactexist = true;
            }
        }
    }

    //private async Task Edit()
    //{
    //    _contactexist = false;
    //    var result = await ContactInformationService.GetEmployerWebContact(UserAccountService.GetUserSKClaim(), _employerSK?.Id ?? 0, 4);
    //    if (result != null)
    //    {
    //        Model = result;
    //        // _contactexist = true;


    //    }
    //}
}
