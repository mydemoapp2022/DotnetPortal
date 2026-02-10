namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// SSN validation result
/// </summary>
public class SSNValidationResult
{
    /// <summary>
    /// IsValid 
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
