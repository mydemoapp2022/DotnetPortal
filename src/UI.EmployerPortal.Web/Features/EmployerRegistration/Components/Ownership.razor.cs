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

    /// <summary>
    /// Step 1: Has the registrant paid employees for work in Wisconsin?
    /// </summary>
    [Parameter]
    public bool HasPaidEmployeesInWI { get; set; }

    /// <summary>
    /// Step 1: Does the registrant expect to pay wages in the future?
    /// </summary>
    [Parameter]
    public bool ExpectsFuturePayroll { get; set; }

    private OwnershipType SelectedOwnershipType { get; set; } = OwnershipType.None;

    private List<string> ValidationErrors { get; set; } = new();
    private List<string> ValidationFieldIds { get; set; } = new();
    private bool _showValidationSummary = false;
    private object? _childFormData;

    // --- New section refs and data ---
    private CorporateOfficerServicesSection? _officerServicesSectionRef;
    private LlcDocumentationSection? _llcDocSectionRef;
    private QualifiedSettlementFundSection? _qsfSectionRef;
    private CorporateOfficerServicesData _corporateOfficerServicesData = new();
    private LlcDocumentationData _llcDocumentationData = new();
    private QualifiedSettlementFundModel _qsfData = new();
    private Func<List<ValidationItem>>? _officerServicesValidateCallback;
    private Func<List<ValidationItem>>? _llcDocValidateCallback;
    private Func<List<ValidationItem>>? _qsfValidateCallback;

    /// <summary>
    /// Show the Corporate Officer Services (UCT-10056-E) section when:
    /// - Ownership = Corporation or LLCCorporation
    /// - Step 1: HavePaid = No AND ExpectFuture = No
    /// </summary>
    private bool ShowCorporateOfficerServicesSection => Model.OwnershipType is (OwnershipType.Corporation or OwnershipType.LLCCorporation)
                                                        && !HasPaidEmployeesInWI
                                                        && !ExpectsFuturePayroll;

    /// <summary>
    /// Show the LLC Documentation section when ownership = LLCCorporation
    /// </summary>
    private bool ShowLlcDocumentationSection => Model.OwnershipType == OwnershipType.LLCCorporation;

    /// <summary>
    /// Show the Qualified Settlement Fund (QSF) section when:
    /// - Ownership = QSF
    /// - Step 1: registrant has paid or will pay wages
    /// </summary>
    private bool ShowQsfSection => Model.OwnershipType == OwnershipType.QSF
                                   && (HasPaidEmployeesInWI || ExpectsFuturePayroll);

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        EditContext = new EditContext(Model);
        RestoreSectionData();
    }

    /// <summary>
    /// Called by parent wizard for validation
    /// </summary>
    public async Task<bool> Validate()
    {
        _showValidationSummary = true;

        // Clear both lists up-front so they stay in sync across repeated calls
        ValidationErrors.Clear();
        ValidationFieldIds.Clear();

        // Ownership type validation (inserts at index 0 if None)
        EnsureOwnershipTypeValidationError();

        // Main ownership form validation (returns messages only, no field IDs)
        if (_validateCallback != null)
        {
            foreach (var msg in _validateCallback().Distinct())
            {
                if (!ValidationErrors.Contains(msg))
                {
                    ValidationErrors.Add(msg);
                    ValidationFieldIds.Add(string.Empty);
                }
            }
        }

        // Aggregate all visible section validations
        AggregateSectionErrors(ShowLlcDocumentationSection, _llcDocValidateCallback);
        AggregateSectionErrors(ShowCorporateOfficerServicesSection, _officerServicesValidateCallback);
        AggregateSectionErrors(ShowQsfSection, _qsfValidateCallback);

        _showValidationSummary = !IsFormValid();
        await InvokeAsync(StateHasChanged);
        return IsFormValid();
    }

    /// <summary>
    /// Merges a section's validation items into the aggregate error/field-ID lists,
    /// keeping both lists in sync and de-duplicating by message text.
    /// </summary>
    private void AggregateSectionErrors(bool showSection, Func<List<ValidationItem>>? validateCallback)
    {
        if (!showSection || validateCallback is null)
        {
            return;
        }

        foreach (var item in validateCallback())
        {
            if (!ValidationErrors.Contains(item.Message))
            {
                ValidationErrors.Add(item.Message);
                ValidationFieldIds.Add(item.FieldId);
            }
        }
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
            _officerServicesValidateCallback = null;
            _llcDocValidateCallback = null;
            _qsfValidateCallback = null;

            // Reset section data when ownership type changes
            _corporateOfficerServicesData = new();
            _llcDocumentationData = new();
            _qsfData = new();
            Model.CorporateOfficerServices = null;
            Model.LlcDocumentation = null;
            Model.QualifiedSettlementFund = null;

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
                case OwnershipType.LLCCorporation:
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

    // --- New section data handlers ---

    private void OnCorporateOfficerServicesChanged(CorporateOfficerServicesData data)
    {
        _corporateOfficerServicesData = data;
        Model.CorporateOfficerServices = data;
    }

    private void OnLlcDocumentationChanged(LlcDocumentationData data)
    {
        _llcDocumentationData = data;
        Model.LlcDocumentation = data;
    }

    private void OnQsfDataChanged(QualifiedSettlementFundModel data)
    {
        _qsfData = data;
        Model.QualifiedSettlementFund = data;
    }

    private void RegisterOfficerServicesValidateCallback(Func<List<ValidationItem>> callback)
    {
        _officerServicesValidateCallback = callback;
    }

    private void RegisterLlcDocValidateCallback(Func<List<ValidationItem>> callback)
    {
        _llcDocValidateCallback = callback;
    }

    private void RegisterQsfValidateCallback(Func<List<ValidationItem>> callback)
    {
        _qsfValidateCallback = callback;
    }

    /// <summary>
    /// Restores section data from the model (for back-navigation/session continuity).
    /// </summary>
    private void RestoreSectionData()
    {
        if (Model.CorporateOfficerServices != null)
        {
            _corporateOfficerServicesData = Model.CorporateOfficerServices;
        }

        if (Model.LlcDocumentation != null)
        {
            _llcDocumentationData = Model.LlcDocumentation;
        }

        if (Model.QualifiedSettlementFund != null)
        {
            _qsfData = Model.QualifiedSettlementFund;
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
    private void RegisterValidateCallback(Func<List<string>> callback)
    {
        _validateCallback = callback;
    }
}
