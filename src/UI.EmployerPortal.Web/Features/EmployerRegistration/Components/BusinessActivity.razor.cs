using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Components;

/// <summary>
/// BusinessActivity page component
/// </summary>
public partial class BusinessActivity : ComponentBase
{
    /// <summary>
    /// Business activity form model
    /// </summary>
    [Parameter] public BusinessActivityModel Model { get; set; } = new();

    private bool _showValidationSummary = false;
    private bool _isSessionLoaded = false;
    private bool _showConstructionWarning = false;

    private List<string> ValidationErrors { get; set; } = [];
    private HashSet<string> InvalidFields { get; set; } = [];
    private Dictionary<string, string> FieldErrors { get; set; } = [];

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ProtectedSessionStorage SessionStorage { get; set; } = default!;

    /// <summary>
    /// OnBackClicked
    /// </summary>
    [Parameter]
    public EventCallback OnBackClicked { get; set; }

    /// <summary>
    /// OnSaveAndQuitClicked
    /// </summary>
    [Parameter]
    public EventCallback OnSaveAndQuitClicked { get; set; }

    /// <summary>
    /// OnContinueClicked
    /// </summary>
    [Parameter]
    public EventCallback OnContinueClicked { get; set; }

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
        ValidateForm();
    }

    /// <summary>
    /// Check if construction warning should be displayed
    /// </summary>
    private void CheckConstructionWarning()
    {
        _showConstructionWarning = Model.PrincipalBusinessActivity.IsConstructionRelated();
    }

    private void AddFieldError(string fieldKey, string message)
    {
        ValidationErrors.Add(message);
        InvalidFields.Add(fieldKey);
        FieldErrors[fieldKey] = message;
    }

    private void ValidateForm()
    {
        ValidationErrors.Clear();
        InvalidFields.Clear();
        FieldErrors.Clear();

        if (!Model.DateBusinessStarted.HasValue)
        {
            AddFieldError("DateBusinessStarted", "Date business started or acquired is required");
        }

        if (!Model.DateFirstPaidEmployeesInWI.HasValue)
        {
            AddFieldError("DateFirstPaidEmployeesInWI", "Date you first had paid employees in WI is required");
        }

        if (!Model.DateFirstPaidWagesInWI.HasValue)
        {
            AddFieldError("DateFirstPaidWagesInWI", "Date first paid wages in WI is required");
        }

        if (Model.PrincipalBusinessActivity == PrincipalBusinessActivityType.None)
        {
            AddFieldError("PrincipalBusinessActivity", "Principal Business Activity is required");
        }

        if (string.IsNullOrWhiteSpace(Model.PrimaryBusinessActivityDescription))
        {
            AddFieldError("PrimaryBusinessActivityDescription", "Primary Business Activity Description is required");
        }

        // Auto-copy primary description if checkbox is checked
        if (Model.SameAsPrimaryBusinessActivity)
        {
            Model.WisconsinSpecificBusinessActivity = Model.PrimaryBusinessActivityDescription;
        }

        StateHasChanged();
    }

    /// <summary>
    /// Called by wizard to trigger validation externally
    /// </summary>
    public async Task<bool> Validate()
    {
        ValidateForm();

        if (ValidationErrors.Any())
        {
            _showValidationSummary = true;
            await InvokeAsync(StateHasChanged);
            return false;
        }

        _showValidationSummary = false;
        await SaveToSession();
        await InvokeAsync(StateHasChanged);
        return true;
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
