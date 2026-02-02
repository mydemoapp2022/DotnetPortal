using Microsoft.AspNetCore.Components;

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

    private string OwnershipType { get; set; } = "sole-proprietor";
    private bool IsOutsideUSA { get; set; }
    private string FirstName { get; set; } = "Joseph";
    private string MiddleInitial { get; set; } = "J";
    private string LastName { get; set; } = "Goyette";
    private string SSN { get; set; } = "123456789";
    private int OwnershipPercentage { get; set; } = 70;
    private bool ShowSSN { get; set; } = false;

    private void ToggleSSNVisibility()
    {
        ShowSSN = !ShowSSN;
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
        // Validate form before continuing
        if (ValidateForm())
        {
            await OnContinueClicked.InvokeAsync();
        }
    }

    private bool ValidateForm()
    {
        // Add validation logic
        return !string.IsNullOrWhiteSpace(OwnershipType) &&
               !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               !string.IsNullOrWhiteSpace(SSN) &&
               OwnershipPercentage > 0 && OwnershipPercentage <= 100;
    }
}
