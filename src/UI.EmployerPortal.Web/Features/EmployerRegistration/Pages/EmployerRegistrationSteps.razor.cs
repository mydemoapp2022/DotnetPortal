namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Pages;

using global::UI.EmployerPortal.Razor.SharedComponents.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Components;


/// <summary>
/// Code-behind for the TaxReportOnly wizard page.
/// </summary>
public partial class EmployerRegistrationSteps
{


    /* [Inject]
     private IDashboardOrchestrator DashboardOrchestrator { get; set; } = default!;*/

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private EmployerRegistrationModelStore ModelStore { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;


    private int _currentStep = 1;
    private int _lastStep = 0;

    private PreliminaryQuestions? _preliminaryQuestionsRef;
    private Ownership? _ownershipRef;
    private BusinessInformation? _businessInformationRef;
    private BusinessContact? _businessContactRef;
    private BusinessActivity? _businessActivityRef;
    private UISubjectivity? _uiSubjectivityRef;
    private Verification? _verificationRef;


    private readonly List<WizardStep> _wizardSteps = new()
    {
        new() { StepNumber = 1, Icon = "icons/employer_reg_step_1.svg", ActionButtonText = "CONTINUE"      },
        new() { StepNumber = 2, Icon = "icons/employer_reg_step_2.svg", ActionButtonText = "CONTINUE"      },
        new() { StepNumber = 3, Icon = "icons/employer_reg_step_3.svg", ActionButtonText = "CONTINUE"      },
        new() { StepNumber = 4, Icon = "icons/employer_reg_step_4.svg", ActionButtonText = "CONTINUE"      },
        new() { StepNumber = 5, Icon = "icons/employer_reg_step_5.svg", ActionButtonText = "CONTINUE"      },
        new() { StepNumber = 6, Icon = "icons/employer_reg_step_6.svg", ActionButtonText = "CONTINUE"      },
        new() { StepNumber = 7, Icon = "icons/employer_reg_step_7.svg", ActionButtonText = "SUBMIT" , BackButtonText = "Return To Registration"}
    };

    /// ── Single source of truth for the page ───────────────────────────────────

    /// <summary>Restores the current step from state (e.g., after returning from the Address Correction page).</summary>
    protected override void OnInitialized()
    {
        if (RegistrationState.CurrentStep > 0)
        {
            _currentStep = RegistrationState.CurrentStep;
            RegistrationState.CurrentStep = 0;
        }
    }

    private async Task HandleVerificationEditClick(int stepNumber)
    {
        _lastStep = stepNumber - 1;
        _currentStep = stepNumber;
        await JSRuntime.InvokeVoidAsync("scrollToTop");

    }

    private async Task HandleActionClick()
    {
        var isValid = await Validate();

        if (!isValid)
        {
            return;
        }

        if (_currentStep == _wizardSteps.Count)
        {
            await HandleSubmit();
            return;
        }
        _currentStep++;
        _lastStep++;
    }

    private async Task HandleSaveAndQuit()
    {
        var isValid = await Validate();

        if (!isValid)
        {
            return;
        }

        _lastStep++;
        _currentStep = _wizardSteps.Count;
    }

    private async Task HandleBackClick()
    {
        if (_currentStep == 1)
        {
            //Delegate validation to Preliminary Questions component
            Nav.NavigateTo("employer-registration/employer-registration-welcome");
        }

        if (_currentStep < _wizardSteps.Count)
        {
            _currentStep--;
            _lastStep = _currentStep - 1;
        }

        if (_currentStep == _wizardSteps.Count)
        {
            _currentStep = _lastStep;
            _lastStep = _currentStep - 1;
        }
    }

    private async Task HandleSubmit()
    {
        // WCF Service calls to save and register
        if (_currentStep == _wizardSteps.Count)
        {
            ModelStore.Save();
        }
        else
        {
            ModelStore.SavePartial(_lastStep);
        }
    }

    private async Task<bool> Validate()
    {
        bool isValid;

        if (_currentStep == 1)
        {
            //Delegate validation to Preliminary Questions component
            isValid = _preliminaryQuestionsRef?.Validate() ?? false;
        }

        else if (_currentStep == 2)
        {
            //Delegate validation to Ownership component
            isValid = await (_ownershipRef?.Validate() ?? Task.FromResult(false));
        }

        else if (_currentStep == 3)
        {
            //Delegate validation to Ownership component
            isValid = await (_businessInformationRef?.Validate() ?? Task.FromResult(false));
        }

        else if (_currentStep == 4)
        {
            //Delegate validation to Ownership component
            isValid = await (_businessContactRef?.Validate() ?? Task.FromResult(false));
        }

        else if (_currentStep == 5)
        {
            //Delegate validation to Ownership component
            isValid = await (_businessActivityRef?.Validate() ?? Task.FromResult(false));
        }

        else if (_currentStep == 6)
        {
            //Delegate validation to Ownership component
            isValid = await (_uiSubjectivityRef?.Validate() ?? Task.FromResult(false));
        }

        else
        {
            // Delegate validation to Step 7
            // Step 7 is Verification, if they clicked submit, they are validating the screen.
            isValid = true;
        }

        return isValid;
    }
}
