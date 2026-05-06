namespace UI.EmployerPortal.Web.Features.BillingPayments.Models;

/// <summary>
/// Specifies the steps in the ACH (Automated Clearing House) processing workflow.
/// </summary>
public enum AchStep
{
    /// <summary>Initial step where the user enters their bank account information.</summary>
    Entry = 0,
    /// <summary>Step where the system verifies the bank account information provided by the user.</summary>
    VerifyAuthorize = 1,
    /// <summary>Final step where the user confirms the ACH payment after successful verification.</summary>
    Confirmation = 2
}
