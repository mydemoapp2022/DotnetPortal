using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [CascadingParameter]
    public EditContext EditContext { get; set; } = default!;

    /// <summary>
    /// Model Injection
    /// </summary>
    [Parameter]
    public OwnershipSessionData Model { get; set; } = default!;

    private OwnershipType SelectedOwnershipType { get; set; } = OwnershipType.None;

    private List<string> ValidationErrors { get; set; } = new();
    private List<string> ValidationFieldIds { get; set; } = new();
    //private bool _shouldValidate = false;
    private bool _showValidationSummary = false;
    //private bool IsContinueEnabled = true;>// IsFormValid();
    private object? _childFormData;
    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        EditContext = new EditContext(Model);
    }

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
            ValidationErrors = _validateCallback().Distinct().ToList();
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
        { OwnershipType.LLCCorporation, typeof(MemberBasedOwnershipForm) },
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
                MemberLabel = "Member",
                StateLabel = "Registration State",
                RequiresState = true,
                MaxEntries = 5,
                InstructionText = "Enter the names, Social Security Numbers, and ownership percentages of the members. If there are more than five members, list the five with the highest ownership percentages."

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

    private void EnsureOwnershipTypeValidationError()
    {
        var requiredMessage = "Ownership Type is required.";
        if (Model.OwnershipType == OwnershipType.None)
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
            _childFormData = null;
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
        return Model.OwnershipType == OwnershipType.None || !_configurationMap.ContainsKey(Model.OwnershipType)
            ? null
            : _configurationMap[Model.OwnershipType];
    }

    private Dictionary<string, object> GetComponentParameters()
    {
        var config = GetCurrentConfig();
        var parameters = new Dictionary<string, object>();

        if (config != null)
        {
            parameters.Add("Config", config);
            parameters.Add("OnValidationChanged", EventCallback.Factory.Create<List<ValidationItem>>(this, OnChildValidationChanged));
            parameters.Add("RegisterValidate", (Action<Func<List<string>>>) RegisterValidateCallback);
            parameters.Add("ContinueData", Model);
            // Create correctly-typed EventCallback to match each child component's parameter type
            switch (Model.OwnershipType)
            {
                case OwnershipType.LLC:
                case OwnershipType.LLP:
                case OwnershipType.LLCCorporation:
                case OwnershipType.Partnership:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<MemberBasedFormData>(this, OnFormDataChanged));
                    break;

                case OwnershipType.LP:
                    parameters.Add("OnDataChanged",
                        EventCallback.Factory.Create<LimitedPartnershipFormData>(this, OnFormDataChanged));
                    break;

                case OwnershipType.Corporation:
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
        }

        return parameters;
    }

    private void OnChildValidationChanged(List<ValidationItem> items)
    {
        ValidationErrors = items.Select(i =>
        {
            return i.Message;
        }).Distinct().ToList();
        ValidationFieldIds = items.Select(i =>
        {
            return i.FieldId;
        }).Distinct().ToList();
        StateHasChanged();
    }

    private void OnFormDataChanged(object formData)
    {
        _childFormData = formData;
        MoveChildDataToModel();
    }

    private void MoveChildDataToModel()
    {
        // Save form-specific data based on ownership type
        if (_childFormData != null)
        {
            switch (Model.OwnershipType)
            {
                case OwnershipType.LLC:
                case OwnershipType.LLP:
                case OwnershipType.Partnership:
                    if (_childFormData is MemberBasedFormData memberData)
                    {
                        Model.RegistrationState = memberData.RegistrationState;
                        Model.Members = memberData.Members;
                        Model.MoreThanFive = memberData.MoreThanFive;
                    }
                    break;

                case OwnershipType.LP:
                    if (_childFormData is LimitedPartnershipFormData lpData)
                    {
                        Model.RegistrationState = lpData.RegistrationState;
                        Model.LimitedPartnershipName = lpData.LimitedPartnershipName;
                        Model.GeneralPartner = lpData.GeneralPartner;
                    }
                    break;

                case OwnershipType.Corporation:
                case OwnershipType.LLCCorporation:
                    if (_childFormData is CorporationFormData corpData)
                    {
                        Model.IncorporationState = corpData.IncorporationState;
                        Model.ForeignCountry = corpData.ForeignCountry;
                        Model.Officers = corpData.Officers;
                    }
                    break;

                case OwnershipType.SoleProprietorship:
                case OwnershipType.Individual:
                    if (_childFormData is OwnerMember owner)
                    {
                        Model.Owner = owner;
                    }
                    break;

                case OwnershipType.Estate:
                    if (_childFormData is EstateFormData estateData)
                    {
                        Model.Decedent = estateData.Decedent;
                        Model.PersonalRepresentative = estateData.PersonalRepresentative;
                    }
                    break;
            }
        }
    }

    private bool IsFormValid()
    {
        // Basic validation - ownership type must be selected
        if (Model.OwnershipType == OwnershipType.None)
        {
            return false;
        }

        // Check if there are any validation errors from child component
        return !ValidationErrors.Any();
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
