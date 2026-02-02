namespace UI.EmployerPortal.Web.Features.Shared.Accounts.Models;

/// <summary>
/// Account for Landing Page
/// </summary>
public sealed record Account
{
    /// <summary>
    /// account Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// account LegalName
    /// </summary>
    public string LegalName { get; init; } = string.Empty;

    /// <summary>
    /// account AccountNo
    /// </summary>
    public string AccountNo { get; init; } = string.Empty;

    /// <summary>
    /// account BalanceDue
    /// </summary>
    public decimal BalanceDue { get; init; }

    /// <summary>
    /// account MissingQuarterlyReports
    /// </summary>
    public int MissingQuarterlyReports { get; init; }

    /// <summary>
    /// account PendingTaxEForms
    /// </summary>
    public int PendingTaxEForms { get; init; }

    /// <summary>
    /// account PendingBenefitsEForms
    /// </summary>
    public int PendingBenefitsEForms { get; init; }

    /// <summary>
    /// account UnreadMessages
    /// </summary>
    public int UnreadMessages { get; init; }
}
