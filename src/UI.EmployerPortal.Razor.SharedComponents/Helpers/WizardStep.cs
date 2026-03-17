namespace UI.EmployerPortal.Razor.SharedComponents.Helpers;

/// <summary>
/// Represents a single step in the wizard component.
/// Optional button text properties override the wizard-level defaults for the step.
/// </summary>
public class WizardStep
{
    /// <summary>
    /// The step number (e.g., 1, 2, 3).
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Optional icon path (e.g., "/icons.svg").
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional override for the Back button text on this step.
    /// </summary>
    public string BackButtonText { get; set; } = "Back";

    /// <summary>
    /// Controls whether the Back buton icon is visible on this step. Default: true
    /// </summary>
    public bool ShowBackButtonIcon { get; set; } = true;

    /// <summary>
    /// Optional override for the Cancel button text on this step.
    /// </summary>
    public string CancelButtonText { get; set; } = "Cancel";

    /// <summary>
    /// Controls whetehr the cancel button border is visible on this step. Default: false
    /// </summary>
    public bool ShowCancelButtonBorder { get; set; } = false;

    /// <summary>
    /// Optional override for the Save and Quit button text on this step.
    /// </summary>
    public string SaveAndQuitButtonText { get; set; } = "Save & Quit";

    /// <summary>
    /// Controls whetehr the SaveAndQuit button border is visible on this step. Default: false
    /// </summary>
    public bool ShowSaveAndQuitButtonBorder { get; set; } = false;

    /// <summary>
    /// Optional override for the Action button text on this step.
    /// </summary>
    public string ActionButtonText { get; set; } = "Submit";

    /// <summary>
    /// Controls whether the Back buton icon is visible on this step. Default: true
    /// </summary>
    public bool ShowActionButtonIcon { get; set; } = true;
}
