using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Pages;

/// <summary>
/// BusinessActivity page component
/// </summary>
public partial class BusinessActivity : ComponentBase
{
    /// <summary>
    /// Business activity form model
    /// </summary>
    private BusinessActivityModel Model { get; set; } = new();

    private bool _showValidationSummary = false;
    private readonly List<string> _validationErrors = new();
    private bool _isSessionLoaded = false;
    private bool _showConstructionWarning = false;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ProtectedSessionStorage SessionStorage { get; set; } = default!;

    /// <summary>
    /// Load session data after first render
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadFromSession();
            _isSessionLoaded = true;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Load saved form data from session storage
    /// </summary>
    private async Task LoadFromSession()
    {
        try
        {
            var result = await SessionStorage.GetAsync<BusinessActivitySessionData>("BusinessActivityData");
            if (result.Success && result.Value != null)
            {
                var savedData = result.Value;
                Model.DateBusinessStarted = savedData.DateBusinessStarted;
                Model.DateFirstPaidEmployeesInWI = savedData.DateFirstPaidEmployeesInWI;
                Model.DateFirstPaidWagesInWI = savedData.DateFirstPaidWagesInWI;
                Model.PrincipalBusinessActivity = savedData.PrincipalBusinessActivity;
                Model.PrimaryBusinessActivityDescription = savedData.PrimaryBusinessActivityDescription;
                Model.SameAsPrimaryBusinessActivity = savedData.SameAsPrimaryBusinessActivity;
                Model.WisconsinSpecificBusinessActivity = savedData.WisconsinSpecificBusinessActivity;

                // Check if we need to show construction warning
                CheckConstructionWarning();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from session: {ex.Message}");
        }
    }

    /// <summary>
    /// Save form data to session storage
    /// </summary>
    private async Task SaveToSession()
    {
        try
        {
            var sessionData = new BusinessActivitySessionData
            {
                DateBusinessStarted = Model.DateBusinessStarted,
                DateFirstPaidEmployeesInWI = Model.DateFirstPaidEmployeesInWI,
                DateFirstPaidWagesInWI = Model.DateFirstPaidWagesInWI,
                PrincipalBusinessActivity = Model.PrincipalBusinessActivity,
                PrimaryBusinessActivityDescription = Model.PrimaryBusinessActivityDescription,
                SameAsPrimaryBusinessActivity = Model.SameAsPrimaryBusinessActivity,
                WisconsinSpecificBusinessActivity = Model.WisconsinSpecificBusinessActivity
            };

            await SessionStorage.SetAsync("BusinessActivityData", sessionData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to session: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle principal activity selection change
    /// </summary>
    private void OnPrincipalActivityChanged()
    {
        CheckConstructionWarning();
        StateHasChanged();
    }

    /// <summary>
    /// Check if construction warning should be displayed
    /// </summary>
    private void CheckConstructionWarning()
    {
        _showConstructionWarning = Model.PrincipalBusinessActivity.IsConstructionRelated();
    }

    private void HandleContinue()
    {
        _validationErrors.Clear();
        _showValidationSummary = false;

        // Auto-copy primary description if checkbox is checked
        if (Model.SameAsPrimaryBusinessActivity)
        {
            Model.WisconsinSpecificBusinessActivity = Model.PrimaryBusinessActivityDescription;
        }

        // Save to session before continuing
        _ = SaveToSession();

        Navigation.NavigateTo("/employer-registration/UISubjectivity");
    }

    private void HandleInvalidSubmit()
    {
        _validationErrors.Clear();
        _showValidationSummary = true;

        // Collect validation errors (optional - add custom logic here)
        if (!Model.DateBusinessStarted.HasValue)
        {
            _validationErrors.Add("Date business started is required");
        }
            

        if (!Model.DateFirstPaidEmployeesInWI.HasValue)
        {
            _validationErrors.Add("Date you first had paid employees in WI is required");
        }


        if (!Model.DateFirstPaidWagesInWI.HasValue)
        {
            _validationErrors.Add("Date first paid wages in WI is required");
        }

        if (string.IsNullOrWhiteSpace(Model.PrimaryBusinessActivityDescription))
        {
            _validationErrors.Add("Primary Business Activity Description is required");
        }   

        StateHasChanged();
    }

    private void HandleBack()
    {
        Navigation.NavigateTo("/employer-registration/BusinessContact");
    }

    private async Task HandleSaveAndQuit()
    {
        // Save current form state to session
        await SaveToSession();

        // Redirect to dashboard
        Navigation.NavigateTo("/");
    }

    /// <summary>
    /// Clear business activity data from session (call after successful submission)
    /// </summary>
    public async Task ClearStoredData()
    {
        try
        {
            await SessionStorage.DeleteAsync("BusinessActivityData");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing storage: {ex.Message}");
        }
    }
}
