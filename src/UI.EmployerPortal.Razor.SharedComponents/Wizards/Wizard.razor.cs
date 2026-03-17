using Microsoft.AspNetCore.Components;
using UI.EmployerPortal.Razor.SharedComponents.Helpers;

namespace UI.EmployerPortal.Razor.SharedComponents.Wizards;

/// <summary>
/// Generic wizard shell component that handles stepper and navigation.
/// </summary>
public partial class Wizard : ComponentBase
{
    /// <summary>
    /// IsVisible
    /// </summary>
    [Parameter]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Collection of steps to display in the wizard.
    /// </summary>
    [Parameter]
    public IEnumerable<WizardStep>? Steps { get; set; }

    /// <summary>
    /// The currently active step number (1-based).
    /// </summary>
    [Parameter]
    public int CurrentStep { get; set; } = 1;

    /// <summary>
    /// Optional content to be rendered below the wizard steps.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS class to apply to the wizard container.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Callback invoked when a step is clicked (optional for navigation).
    /// </summary>
    [Parameter]
    public EventCallback<int> OnStepClicked { get; set; }

    /// <summary>
    /// Controls whether navigation buttons (Back, Cancel, Continue) are displayed.
    /// </summary>
    [Parameter]
    public bool ShowNavigationButtons { get; set; } = true;

    /// <summary>
    /// Controls whether the Back button is displayed.
    /// </summary>
    [Parameter]
    public bool ShowBackButton { get; set; } = true;

    /// <summary>
    /// Controls whether the Back button is disabled in the first step.
    /// </summary>
    [Parameter]
    public bool DisableBackOnFirstStep { get; set; } = true;

    /// <summary>
    /// Controls whether the Cancel button is displayed.
    /// </summary>
    [Parameter]
    public bool ShowCancelButton { get; set; } = true;

    /// <summary>
    /// Controls whether the Savd and Quit button is displayed.
    /// </summary>
    [Parameter]
    public bool ShowSaveAndQuitButton { get; set; } = true;

    /// <summary>
    /// Controls whether the submit button is displayed on the last step.
    /// </summary>
    [Parameter]
    public bool ShowActionButton { get; set; } = true;

    /// <summary>
    /// Callback invoked when the Back button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnBackClick { get; set; }

    /// <summary>
    /// Callback invoked when the Cancel button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnCancelClick { get; set; }

    /// <summary>
    /// Callback invoked when the Save and Quit button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnSaveAndQuitClick { get; set; }

    /// <summary>
    /// Callback invoked when the Submit Report button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnActionClick { get; set; }

    /// <summary>
    /// If true, automatically navigates to the previous step when Back is clicked.
    /// </summary>
    [Parameter]
    public bool AutoNavigateOnBack { get; set; } = true;

    /// <summary>
    /// If true, automatically navigates to the next step when Continue is clicked.
    /// </summary>
    [Parameter]
    public bool AutoNavigateOnAction { get; set; } = true;

    /// <summary>
    /// Two-way bindable parameter for CurrentStep to support automatic navigation.
    /// </summary>
    [Parameter]
    public EventCallback<int> CurrentStepChanged { get; set; }

    /// <summary>
    /// Gets the data for the current step based on the CurrentStep number.
    /// </summary>
    public WizardStep? CurrentStepData => Steps?.FirstOrDefault(s =>
    {
        return s.StepNumber == CurrentStep;
    });

    internal string ResolvedBackButtonText => CurrentStepData?.BackButtonText ?? "Back";
    internal bool ResolvedShowBackButtonIcon => CurrentStepData?.ShowBackButtonIcon ?? true;
    internal string ResolvedCancelButtonText => CurrentStepData?.CancelButtonText ?? "Cancel";
    internal bool ResolvedShowCancelButtonBorder => CurrentStepData?.ShowCancelButtonBorder ?? false;
    internal string ResolvedSaveAndQuitButtonText => CurrentStepData?.SaveAndQuitButtonText ?? "Save & Quit";
    internal bool ResolvedShowSaveAndQuitButtonBorder => CurrentStepData?.ShowSaveAndQuitButtonBorder ?? false;
    internal string ResolvedActionButtonText => CurrentStepData?.ActionButtonText ?? "Submit";
    internal bool ResolvedShowActionButtonIcon => CurrentStepData?.ShowActionButtonIcon ?? true;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await SetWizardSteps();
    }

    internal async Task SetWizardSteps()
    {
        if (Steps != null && Steps.Any())
        {
            var maxStep = Steps.Max(s =>
            {
                return s.StepNumber;
            });

            if (CurrentStep < 1)
            {
                CurrentStep = 1;
            }
            else if (CurrentStep > maxStep)
            {
                CurrentStep = maxStep;
            }
        }

    }

    /// <summary>
    /// Handles step click events if navigation is enabled.
    /// </summary>
    internal async Task HandleStepClick(int stepNumber)
    {
        if (OnStepClicked.HasDelegate)
        {
            await OnStepClicked.InvokeAsync(stepNumber);
        }
    }

    /// <summary>
    /// Handles the Back button click.
    /// </summary>
    internal async Task HandleBackClick()
    {
        if (OnBackClick.HasDelegate)
        {
            await OnBackClick.InvokeAsync();
        }

        if (AutoNavigateOnBack && CurrentStep > 1)
        {
            CurrentStep--;
            await CurrentStepChanged.InvokeAsync(CurrentStep);
        }
    }

    /// <summary>
    /// Handles the Cancel button click.
    /// </summary>
    internal async Task HandleCancelClick()
    {
        if (OnCancelClick.HasDelegate)
        {
            await OnCancelClick.InvokeAsync();
        }
    }

    /// <summary>
    /// Handles the Continue button click.
    /// </summary>
    internal async Task HandleActionClick()
    {
        if (OnActionClick.HasDelegate)
        {
            await OnActionClick.InvokeAsync();
        }

        if (AutoNavigateOnAction && CurrentStep < (Steps?.Count() ?? 0))
        {
            CurrentStep++;
            await CurrentStepChanged.InvokeAsync(CurrentStep);
        }
    }

    /// <summary>
    /// Handles the Save and Quit button click.
    /// </summary>
    internal async Task HandleSaveAndQuitClick()
    {
        if (OnSaveAndQuitClick.HasDelegate)
        {
            await OnSaveAndQuitClick.InvokeAsync();
        }
    }
}
