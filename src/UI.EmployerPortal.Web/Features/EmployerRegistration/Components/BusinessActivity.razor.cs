using Microsoft.AspNetCore.Components;
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

    /// <summary>
    /// The ownership type selected on a previous step, used for governmental employer checks.
    /// </summary>
    [Parameter] public OwnershipType OwnershipType { get; set; } = OwnershipType.None;

    /// <summary>
    /// Whether the employer indicated they have paid employees in WI (from page 1 preliminary questions).
    /// </summary>
    [Parameter] public bool HasPaidEmployeesInWI { get; set; } = true;

    private bool _showValidationSummary = false;
    private bool _showConstructionWarning = false;

    /// <summary>
    /// When true, forces all child inputs to display their errors regardless of
    /// individual touch state (set on Continue / form submit).
    /// Matches the Visible parameter pattern used by OutlinedTextField.
    /// </summary>
    private bool _showAllErrors = false;

    private const int MaxDescriptionLength = 255;

    private List<string> ValidationErrors { get; set; } = [];
    private List<string> ValidationFieldIds { get; set; } = [];
    private HashSet<string> InvalidFields { get; set; } = [];
    private Dictionary<string, string> FieldErrors { get; set; } = [];

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

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
    /// Handle principal activity selection change.
    /// Clears description fields so stale text from a previous selection is not submitted.
    /// </summary>
    private void OnPrincipalActivityChanged()
    {
        Model.PrimaryBusinessActivityDescription = string.Empty;
        Model.WisconsinSpecificBusinessActivity = string.Empty;

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

    /// <summary>
    /// Maps a field key to its HTML element id for NotificationBanner error links.
    /// </summary>
    private static string GetElementId(string fieldKey)
    {
        return fieldKey switch
        {
            "DateBusinessStarted" => "dateStarted",
            "DateFirstPaidEmployeesInWI" => "dateFirstPaid",
            "DateFirstPaidWagesInWI" => "dateFirstWages",
            "PrincipalBusinessActivity" => "principal-activity",
            "PrimaryBusinessActivityDescription" => "primaryDescription",
            "WisconsinSpecificBusinessActivity" => "wiDescription",
            _ => string.Empty
        };
    }

    private void AddFieldError(string fieldKey, string message)
    {
        ValidationErrors.Add(message);
        ValidationFieldIds.Add(GetElementId(fieldKey));
        InvalidFields.Add(fieldKey);
        FieldErrors[fieldKey] = message;
    }

    private void ValidateForm()
    {
        ValidationErrors.Clear();
        ValidationFieldIds.Clear();
        InvalidFields.Clear();
        FieldErrors.Clear();

        // --- Date Business Started ---
        if (!Model.DateBusinessStarted.HasValue)
        {
            AddFieldError("DateBusinessStarted", "Date business started or acquired is required.");
        }
        else if (Model.DateBusinessStarted.Value.Date > DateTime.Today)
        {
            AddFieldError("DateBusinessStarted", "Date business started must be today or earlier.");
        }

        // --- Date First Employees in WI ---
        if (!Model.DateFirstPaidEmployeesInWI.HasValue)
        {
            AddFieldError("DateFirstPaidEmployeesInWI", "Date you first had paid employees working in WI is required.");
        }
        else if (Model.DateFirstPaidEmployeesInWI.Value.Date > DateTime.Today)
        {
            AddFieldError("DateFirstPaidEmployeesInWI", "Date you first had employees working in Wisconsin must be today or earlier.");
        }

        // --- Date First Paid Wages in WI ---
        if (!Model.DateFirstPaidWagesInWI.HasValue)
        {
            AddFieldError("DateFirstPaidWagesInWI", "Date first paid wages for work performed in WI is required.");
        }
        else
        {
            var wagesDate = Model.DateFirstPaidWagesInWI.Value.Date;

            // Must be between Date Business Started and today
            if (Model.DateBusinessStarted.HasValue && wagesDate < Model.DateBusinessStarted.Value.Date)
            {
                AddFieldError("DateFirstPaidWagesInWI",
                    "Date you first paid wages for work performed in Wisconsin must be between the date the business started and today OR you need to answer NO to Have you paid employees for work performed in Wisconsin on page 1.");
            }
            else if (wagesDate > DateTime.Today)
            {
                AddFieldError("DateFirstPaidWagesInWI",
                    "Date you first paid wages for work performed in Wisconsin must be between the date the business started and today.");
            }

            // Must be on or after Date First Employees
            if (!FieldErrors.ContainsKey("DateFirstPaidWagesInWI")
                && Model.DateFirstPaidEmployeesInWI.HasValue
                && wagesDate < Model.DateFirstPaidEmployeesInWI.Value.Date)
            {
                AddFieldError("DateFirstPaidWagesInWI",
                    "Date you first paid wages for work performed in Wisconsin must be on or after the date you first had employees working in Wisconsin.");
            }

            // TODO: Payroll quarter check — requires external API data to determine if payroll
            // has been reported prior to the quarter of the entered date.
            // "Payroll has been reported prior to the quarter of first payroll entered,
            //  please update the date you first paid wages for work performed in Wisconsin."
        }

        // --- Principal Business Activity ---
        if (Model.PrincipalBusinessActivity == PrincipalBusinessActivityType.None)
        {
            AddFieldError("PrincipalBusinessActivity", "Principal Business Activity is required.");
        }

        // --- Primary Business Activity Description (max 255 chars) ---
        if (string.IsNullOrWhiteSpace(Model.PrimaryBusinessActivityDescription))
        {
            AddFieldError("PrimaryBusinessActivityDescription", "Primary Business Activity Description is required.");
        }
        else if (Model.PrimaryBusinessActivityDescription.Length > MaxDescriptionLength)
        {
            AddFieldError("PrimaryBusinessActivityDescription",
                $"Primary business activity description cannot exceed {MaxDescriptionLength} characters.");
        }

        // --- Wisconsin Specific Business Activity (max 255 chars) ---
        if (!Model.SameAsPrimaryBusinessActivity)
        {
            if (string.IsNullOrWhiteSpace(Model.WisconsinSpecificBusinessActivity))
            {
                AddFieldError("WisconsinSpecificBusinessActivity", "Wisconsin Specific Business Activity is required.");
            }
            else if (Model.WisconsinSpecificBusinessActivity.Length > MaxDescriptionLength)
            {
                AddFieldError("WisconsinSpecificBusinessActivity",
                    $"Wisconsin specific business activity cannot exceed {MaxDescriptionLength} characters.");
            }
        }

        // Auto-copy primary description if checkbox is checked
        if (Model.SameAsPrimaryBusinessActivity)
        {
            Model.WisconsinSpecificBusinessActivity = Model.PrimaryBusinessActivityDescription;
        }
    }

    /// <summary>
    /// Called by wizard to trigger validation externally
    /// </summary>
    public async Task<bool> Validate()
    {
        _showValidationSummary = true;
        _showAllErrors = true;
        ValidateForm();

        if (ValidationErrors.Any())
        {
            await InvokeAsync(StateHasChanged);
            return false;
        }

        // --- Government employer business rule check ---
        // If registered date is null AND employer is Governmental AND paid employees = false,
        // then check for Partial Match / Exact Match. If either is true, exit to summary page.
        if (OwnershipType.IsGovernmental() && !HasPaidEmployeesInWI)
        {
            // TODO: Call service to check for Partial Match and Exact Match.
            // If either returns true, navigate to the summary page:
            // Navigation.NavigateTo("/employer-registration/summary");
            // return false;
        }

        _showValidationSummary = false;
        await InvokeAsync(StateHasChanged);
        return true;
    }
}
