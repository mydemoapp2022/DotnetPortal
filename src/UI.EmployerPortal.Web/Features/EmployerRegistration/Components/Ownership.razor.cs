using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components.OwnershipForms;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Components;

/// <summary>
/// Ownership component for employer registration
/// </summary>
public partial class Ownership
{
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

    private string OwnershipType { get; set; } = string.Empty;
    private bool IsOutsideUSA { get; set; }
    private List<string> ValidationErrors { get; set; } = new();
    private bool _shouldValidate = false;
    private bool IsContinueEnabled => IsFormValid();
    private object? _currentFormData;
    private OwnershipSessionData? _savedSessionData;
    private bool _isSessionLoaded;

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

    /// <summary>
    /// OnAfterRenderAsync - Load session data here where JS interop is available
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

    private async Task LoadFromSession()
    {
        try
        {
            var result = await SessionStorage.GetAsync<OwnershipSessionData>("OwnershipData");
            if (result.Success && result.Value != null)
            {
                var savedData = result.Value;
                OwnershipType = savedData.OwnershipType;
                IsOutsideUSA = savedData.IsOutsideUSA;
                _savedSessionData = savedData;

                // Debug logging
                Console.WriteLine($"✅ Session loaded: OwnershipType={savedData.OwnershipType}");
                Console.WriteLine($"   - Members: {savedData.Members?.Count ?? 0}");
                Console.WriteLine($"   - Officers: {savedData.Officers?.Count ?? 0}");
                Console.WriteLine($"   - Owner: {(savedData.Owner != null ? "Yes" : "No")}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from session: {ex.Message}");
        }
    }

    private async Task OnOwnershipTypeChanged()
    {
        try
        {
            // Clear validation errors when ownership type changes
            ValidationErrors.Clear();

            // Clear saved session data when changing ownership type
            // This prevents passing incompatible data to the new component
            _savedSessionData = null;
            _currentFormData = null;

            // Reset validation flag
            _shouldValidate = false;
            StateHasChanged();

            // Wait for the new DynamicComponent to render
            await Task.Delay(100);

            // Trigger validation on the new component
            _shouldValidate = true;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnOwnershipTypeChanged: {ex.Message}");
        }
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

            // Create correctly-typed EventCallback to match each child component's parameter type
            switch (OwnershipType)
            {
                case "llc":
                case "llp":
                case "lp":
                case "partnership":
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<MemberBasedFormData>(this, OnFormDataChanged));
                    break;

                case "corporation":
                case "llc-corporation":
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<CorporationFormData>(this, OnFormDataChanged));
                    break;

                case "sole-proprietorship":
                case "individual":
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<OwnerMember>(this, OnFormDataChanged));
                    break;
            }

            // Only pass saved data if it matches current ownership type
            if (_savedSessionData != null && _savedSessionData.OwnershipType == OwnershipType)
            {
                parameters.Add("SavedData", _savedSessionData);
            }
        }

        return parameters;
    }

    private void OnChildValidationChanged(List<string> errors)
    {
        ValidationErrors = errors ?? new List<string>();
        StateHasChanged();
    }

    private void OnFormDataChanged(object formData)
    {
        _currentFormData = formData;
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
        await SaveToSession();
        await OnSaveAndQuitClicked.InvokeAsync();
    }

    private async Task OnContinue()
    {
        // Save to session before continuing
        await SaveToSession();

        // Navigate to next step - button is only enabled when form is valid
        await OnContinueClicked.InvokeAsync();
    }

    private async Task SaveToSession()
    {
        try
        {
            var sessionData = new OwnershipSessionData
            {
                OwnershipType = OwnershipType,
                IsOutsideUSA = IsOutsideUSA
            };

            // Save form-specific data based on ownership type
            if (_currentFormData != null)
            {
                switch (OwnershipType)
                {
                    case "llc":
                    case "llp":
                    case "lp":
                    case "partnership":
                        if (_currentFormData is MemberBasedFormData memberData)
                        {
                            sessionData.RegistrationState = memberData.RegistrationState;
                            sessionData.Members = memberData.Members;
                            sessionData.MoreThanFive = memberData.MoreThanFive;
                        }
                        break;

                    case "corporation":
                    case "llc-corporation":
                        if (_currentFormData is CorporationFormData corpData)
                        {
                            sessionData.IncorporationState = corpData.IncorporationState;
                            sessionData.ForeignCountry = corpData.ForeignCountry;
                            sessionData.Officers = corpData.Officers;
                        }
                        break;

                    case "sole-proprietorship":
                    case "individual":
                        if (_currentFormData is OwnerMember owner)
                        {
                            sessionData.Owner = owner;
                        }
                        break;
                }
            }

            await SessionStorage.SetAsync("OwnershipData", sessionData);
            _savedSessionData = sessionData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to session: {ex.Message}");
        }
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

