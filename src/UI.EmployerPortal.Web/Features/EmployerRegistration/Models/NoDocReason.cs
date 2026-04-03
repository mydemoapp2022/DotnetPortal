namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Reason why the LLC documentation cannot be submitted at registration time.
/// </summary>
public enum NoDocReason
{
    /// <summary>The LLC has not yet applied to the IRS.</summary>
    HaventApplied,

    /// <summary>The LLC applied but has not received a decision from the IRS.</summary>
    AppliedNoDecision
}
