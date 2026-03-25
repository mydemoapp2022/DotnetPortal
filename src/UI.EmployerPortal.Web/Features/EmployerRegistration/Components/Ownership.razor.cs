using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components.OwnershipForms;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Models;
namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Components;

/// <summary>
/// Ownership component for employer registration
/// </summary>
public partial class Ownership
{
    [Inject]
    private NavigationManager Nav { get; set; } = default!;
    [Inject]
    private ProtectedSessionStorage SessionStorage { get; set; } = default!;
    private IJSRuntime JSRuntime { get; set; } = default!;

    //[Inject]
    //private ProtectedLocalStorage LocalStorage { get; set; } = default!;

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

    private OwnershipType SelectedOwnershipType { get; set; } = OwnershipType.None;
    private bool IsOutsideUSA { get; set; }
    private List<string> ValidationErrors { get; set; } = new();
    private List<string> ValidationFieldIds { get; set; } = new();
    //private bool _shouldValidate = false;
    private bool _showValidationSummary = false;
    //private bool IsContinueEnabled = true;>// IsFormValid();
    private object? _currentFormData;
    private OwnershipSessionData? _savedSessionData;
    private bool _isSessionLoaded;

    /// <summary>
    /// Called by parent wizard for validation
    /// </summary>
    public async Task<bool> Validate()
    {
        //_shouldValidate = true;
        _showValidationSummary = true;
        EnsureOwnershipTypeValidationError();
        if (_validateCallback != null)
        {
            ValidationErrors = _validateCallback();
            EnsureOwnershipTypeValidationError();
        }
        //await InvokeAsync(("RegisterValidate", (Action<Func<List<string>>>) RegisterValidateCallback);
        _showValidationSummary = !IsFormValid();
        await InvokeAsync(StateHasChanged);
        return IsFormValid();
    }

    // Component mapping dictionary
    private readonly Dictionary<OwnershipType, Type> _ownershipComponentMap = new()
    {
        { OwnershipType.Corporation, typeof(CorporationOwnershipForm) },
        { OwnershipType.LLCCorporation, typeof(CorporationOwnershipForm) },
        { OwnershipType.LLC, typeof(MemberBasedOwnershipForm) },
        { OwnershipType.LLP, typeof(MemberBasedOwnershipForm) },
        { OwnershipType.LP, typeof(LimitedPartnershipOwnershipForm) },
        { OwnershipType.Partnership, typeof(MemberBasedOwnershipForm) },
        { OwnershipType.SoleProprietorship, typeof(SoleProprietorshipOwnershipForm) },
        { OwnershipType.Individual, typeof(SoleProprietorshipOwnershipForm) },
        { OwnershipType.Estate, typeof(EstateOwnershipForm) }
    };

    // Configuration mapping using enum
    private readonly Dictionary<OwnershipType, OwnershipFormConfig> _configurationMap = new()
    {
        {
            OwnershipType.Corporation,
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "corporation",
                TypeDisplayName = "Corporation",
                StateLabel = "Incorporation State",
                RequiresForeignCountry = true,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the principal officers.",
                PredefinedRoles = ["President", "Vice President", "Secretary", "Treasurer"]
            }
        },
        {
            OwnershipType.LLCCorporation,
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "llc-corporation",
                TypeDisplayName = "LLC Electing to be Treated as a Corporation",
                StateLabel = "Incorporation State",
                RequiresForeignCountry = true,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the principal officers.",
                PredefinedRoles = ["President", "Vice President", "Secretary", "Treasurer"]
            }
        },
        {
            OwnershipType.LLC,
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
            OwnershipType.LLP,
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
            OwnershipType.LP,
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
            OwnershipType.Partnership,
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
            OwnershipType.SoleProprietorship,
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "sole-proprietorship",
                TypeDisplayName = "Sole Proprietorship (not LLC or Corporation)",
                MaxEntries = 1
            }
        },
        {
            OwnershipType.Individual,
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "individual",
                TypeDisplayName = "Individual",
                MaxEntries = 1
            }
        },
        {
            OwnershipType.Estate,
            new OwnershipFormConfig
            {
                OwnershipTypeValue = "estate",
                TypeDisplayName = "Estate",
                MaxEntries = 2
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
            //var result = await LocalStorage.GetAsync<OwnershipSessionData>("OwnershipData");  // Changed SessionStorage to LocalStorage
            if (result.Success && result.Value != null)
            {
                var savedData = result.Value;
                SelectedOwnershipType = savedData.OwnershipType;
                IsOutsideUSA = savedData.IsOutsideUSA;
                _savedSessionData = savedData;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from session: {ex.Message}");
        }
    }

    private void EnsureOwnershipTypeValidationError()
    {
        var requiredMessage = "Ownership Type is required.";
        if (SelectedOwnershipType == OwnershipType.None)
        {
            if (!ValidationErrors.Contains(requiredMessage))
            {
                ValidationErrors.Insert(0, requiredMessage);
                ValidationFieldIds.Insert(0, "ownership-type");
            }
        }
        else
        {
            var idx = ValidationErrors.IndexOf(requiredMessage);
            if (idx >= 0)
            {
                ValidationErrors.RemoveAt(idx);
                if (idx < ValidationFieldIds.Count)
                {
                    ValidationFieldIds.RemoveAt(idx);
                }   
            }
        }
    }

    private async Task OnOwnershipTypeChanged()
    {
        try
        {

            ValidationErrors.Clear();
            ValidationFieldIds.Clear();
            _showValidationSummary = false;
            _savedSessionData = null;
            _currentFormData = null;
            _validateCallback = null;
            StateHasChanged();
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnOwnershipTypeChanged: {ex.Message}");
        }
    }

    private OwnershipFormConfig? GetCurrentConfig()
    {
        return SelectedOwnershipType == OwnershipType.None || !_configurationMap.ContainsKey(SelectedOwnershipType)
            ? null
            : _configurationMap[SelectedOwnershipType];
    }

    private Dictionary<string, object> GetComponentParameters()
    {
        var config = GetCurrentConfig();
        var parameters = new Dictionary<string, object>();

        if (config != null)
        {
            parameters.Add("Config", config);
            parameters.Add("OnValidationChanged",
                EventCallback.Factory.Create<List<ValidationItem>>(this, OnChildValidationChanged));
            parameters.Add("IsOutsideUSA", IsOutsideUSA);
            parameters.Add("RegisterValidate", (Action<Func<List<string>>>) RegisterValidateCallback);

            switch (SelectedOwnershipType)
            {
                case OwnershipType.LLC:
                case OwnershipType.LLP:
                case OwnershipType.Partnership:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<MemberBasedFormData>(this, OnFormDataChanged));
                    break;

                case OwnershipType.LP:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<LimitedPartnershipFormData>(this, OnFormDataChanged));
                    break;

                case OwnershipType.Corporation:
                case OwnershipType.LLCCorporation:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<CorporationFormData>(this, OnFormDataChanged));
                    break;

                case OwnershipType.SoleProprietorship:
                case OwnershipType.Individual:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<OwnerMember>(this, OnFormDataChanged));
                    break;

                case OwnershipType.Estate:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<EstateFormData>(this, OnFormDataChanged));
                    break;
            }

            // Pass saved data uniformly as SavedData for all component types
            if (_savedSessionData != null && _savedSessionData.OwnershipType == SelectedOwnershipType)
            {
                parameters.Add("SavedData", _savedSessionData);
            }
        }

        return parameters;
    }

    private void OnChildValidationChanged(List<ValidationItem> items)
    {
        ValidationErrors = items.Select(i =>
        {
            return i.Message;
        }).ToList();
        ValidationFieldIds = items.Select(i =>
        {
            return i.FieldId;
        }).ToList();
        StateHasChanged();
    }

    private void OnFormDataChanged(object formData)
    {
        _currentFormData = formData;
    }

    private string GetRequiredFieldsMessage()
    {
        return SelectedOwnershipType switch
        {
            OwnershipType.SoleProprietorship => "• Sole Proprietor First Name is required • Sole Proprietor Last Name is required • SSN is required",
            OwnershipType.Individual => "• Individual First Name is required • Individual Last Name is required • SSN is required",
            OwnershipType.LLC => "• Member 1 First Name is required • Member 1 Last Name is required • Member 1 SSN is required",
            OwnershipType.Corporation or OwnershipType.LLCCorporation => "• Sole Proprietor First Name is required • Sole Proprietor Last Name is required • SSN is required",
            _ => "All fields are required unless noted"
        };
    }

    private async Task SaveToSession()
    {
        try
        {
            var sessionData = new OwnershipSessionData
            {
                OwnershipType = SelectedOwnershipType,
                IsOutsideUSA = IsOutsideUSA
            };

            // Save form-specific data based on ownership type
            if (_currentFormData != null)
            {
                switch (SelectedOwnershipType)
                {
                    case OwnershipType.LLC:
                    case OwnershipType.LLP:
                    case OwnershipType.Partnership:
                        if (_currentFormData is MemberBasedFormData memberData)
                        {
                            sessionData.RegistrationState = memberData.RegistrationState;
                            sessionData.Members = memberData.Members;
                            sessionData.MoreThanFive = memberData.MoreThanFive;
                        }
                        break;

                    case OwnershipType.LP:
                        if (_currentFormData is LimitedPartnershipFormData lpData)
                        {
                            sessionData.RegistrationState = lpData.RegistrationState;
                            sessionData.LimitedPartnershipName = lpData.LimitedPartnershipName;
                            sessionData.GeneralPartner = lpData.GeneralPartner;
                        }
                        break;

                    case OwnershipType.Corporation:
                    case OwnershipType.LLCCorporation:
                        if (_currentFormData is CorporationFormData corpData)
                        {
                            sessionData.IncorporationState = corpData.IncorporationState;
                            sessionData.ForeignCountry = corpData.ForeignCountry;
                            sessionData.Officers = corpData.Officers;
                        }
                        break;

                    case OwnershipType.SoleProprietorship:
                    case OwnershipType.Individual:
                        if (_currentFormData is OwnerMember owner)
                        {
                            sessionData.Owner = owner;
                        }
                        break;

                    case OwnershipType.Estate:
                        if (_currentFormData is EstateFormData estateData)
                        {
                            sessionData.Decedent = estateData.Decedent;
                            sessionData.PersonalRepresentative = estateData.PersonalRepresentative;
                        }
                        break;
                }
            }

            await SessionStorage.SetAsync("OwnershipData", sessionData);
            //await LocalStorage.SetAsync("OwnershipData", sessionData);
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
        if (SelectedOwnershipType == OwnershipType.None)
        {
            return false;
        }

        // Check if there are any validation errors from child component
        return !ValidationErrors.Any();
    }

    /// <summary>
    /// Clear ownership data from local storage (call after successful submission)
    /// </summary>
    public async Task ClearStoredData()
    {
        try
        {
            await SessionStorage.DeleteAsync("OwnershipData");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing storage: {ex.Message}");
        }
    }

    private Func<List<string>>? _validateCallback;
    ///// <summary>
    ///// RegisterValidateCallback
    ///// </summary>
    ///// <param name="callback"></param>
    private void RegisterValidateCallback(Func<List<string>> callback)
    {
        _validateCallback = callback;
    }
}
