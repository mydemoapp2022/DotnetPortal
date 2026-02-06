using Microsoft.AspNetCore.Components;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components.OwnershipForms;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Components;

/// <summary>
/// Ownership component for employer registration
/// </summary>
public partial class Ownership
{
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

    private string OwnershipType { get; set; } = string.Empty;
    private bool IsOutsideUSA { get; set; }
    private List<string> ValidationErrors { get; set; } = new();
    private bool _shouldValidate = false;
    private bool IsContinueEnabled => IsFormValid();

    // Component mapping dictionary
    private readonly Dictionary<string, Type> _ownershipComponentMap = new()
    {
        { "corporation", typeof(CorporationOwnershipForm) },
        { "llc-corporation", typeof(CorporationOwnershipForm) },
        { "llc", typeof(MemberBasedOwnershipForm) },
        { "llp", typeof(MemberBasedOwnershipForm) },
        { "lp", typeof(MemberBasedOwnershipForm) },
        { "partnership", typeof(MemberBasedOwnershipForm) },
        { "sole-proprietorship", typeof(SoleProprietorshipOwnershipForm) },
        { "individual", typeof(SoleProprietorshipOwnershipForm) }
    };

    // Configuration mapping for each ownership type
    private readonly Dictionary<string, OwnershipFormConfig> _configurationMap = new()
    {
        {
            "corporation",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "corporation",
                TypeDisplayName = "Corporation",
                StateLabel = "Incorporation State",
                RequiresForeignCountry = true,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the principal officers.",
                PredefinedRoles = new List<string> { "President", "Vice President", "Secretary", "Treasurer" }
            }
        },
        {
            "llc-corporation",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "llc-corporation",
                TypeDisplayName = "LLC Electing to be Treated as a Corporation",
                StateLabel = "Incorporation State",
                RequiresForeignCountry = true,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the principal officers.",
                PredefinedRoles = new List<string> { "President", "Vice President", "Secretary", "Treasurer" }
            }
        },
        {
            "llc",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "llc",
                TypeDisplayName = "Limited Liability Company (LLC)",
                MemberLabel = "Member",
                StateLabel = "Registration State",
                RequiresState = true,
                MaxEntries = 5,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the members. If there are more than five members, list the five with the highest ownership percentages."
            }
        },
        {
            "llp",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "llp",
                TypeDisplayName = "Limited Liability Partnership (LLP)",
                MemberLabel = "Partner",
                StateLabel = "Registration State",
                RequiresState = true,
                MaxEntries = 5,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the partners. If there are more than five partners, list the five with the highest ownership percentages."
            }
        },
        {
            "lp",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "lp",
                TypeDisplayName = "Limited Partnership (LP)",
                MemberLabel = "Partner",
                StateLabel = "Registration State",
                RequiresState = true,
                MaxEntries = 5,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the partners. If there are more than five partners, list the five with the highest ownership percentages."
            }
        },
        {
            "partnership",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "partnership",
                TypeDisplayName = "Partnership (not LLC or Corporation)",
                MemberLabel = "Partner",
                StateLabel = "Registration State",
                RequiresState = false,
                MaxEntries = 5,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the partners. If there are more than five partners, list the five with the highest ownership percentages."
            }
        },
        {
            "sole-proprietorship",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "sole-proprietorship",
                TypeDisplayName = "Sole Proprietorship (not LLC or Corporation)",
                MaxEntries = 1
            }
        },
        {
            "individual",
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "individual",
                TypeDisplayName = "Individual",
                MaxEntries = 1
            }
        }
    };

    private async void OnOwnershipTypeChanged()
    {
        // Clear validation errors when ownership type changes
        ValidationErrors.Clear();
        // Reset validation flag first
        _shouldValidate = false;
        StateHasChanged();

        // Wait for component to render
        await Task.Delay(50);

        // Trigger validation on the new component
        _shouldValidate = true;
        StateHasChanged();
    }

    private OwnershipFormConfig? GetCurrentConfig()
    {
        return string.IsNullOrEmpty(OwnershipType) || !_configurationMap.ContainsKey(OwnershipType)
            ? null
            : _configurationMap[OwnershipType];
    }

    private Dictionary<string, object> GetComponentParameters()
    {
        var config = GetCurrentConfig();
        var parameters = new Dictionary<string, object>();

        if (config != null)
        {
            parameters.Add("Config", config);
            parameters.Add("OnValidationChanged", EventCallback.Factory.Create<List<string>>(this, OnChildValidationChanged));
            parameters.Add("ShouldValidate", _shouldValidate);
        }

        return parameters;
    }

    private void OnChildValidationChanged(List<string> errors)
    {
        ValidationErrors = errors ?? new List<string>();
        StateHasChanged();
    }

    private string GetRequiredFieldsMessage()
    {
        return OwnershipType switch
        {
            "sole-proprietorship" => "• Sole Proprietor First Name is required • Sole Proprietor Last Name is required • SSN is required",
            "individual" => "• Individual First Name is required • Individual Last Name is required • SSN is required",
            "llc" => "• Member 1 First Name is required • Member 1 Last Name is required • Member 1 SSN is required",
            "corporation" or "llc-corporation" => "• Sole Proprietor First Name is required • Sole Proprietor Last Name is required • SSN is required",
            _ => "All fields are required unless noted"
        };
    }

    private async Task OnBack()
    {
        await OnBackClicked.InvokeAsync();
    }

    private async Task OnSaveAndQuit()
    {
        await OnSaveAndQuitClicked.InvokeAsync();
    }

    private async Task OnContinue()
    {
        // Simply navigate to next step - button is only enabled when form is valid
        await OnContinueClicked.InvokeAsync();
    }

    private bool IsFormValid()
    {
        // Basic validation - ownership type must be selected
        if (string.IsNullOrWhiteSpace(OwnershipType))
        {
            return false;
        }

        // Check if there are any validation errors from child component
        return !ValidationErrors.Any();
    }
}
