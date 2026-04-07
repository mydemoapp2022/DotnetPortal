namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Data for LLC Corporation documentation section (UCT-10334-E)
/// </summary>
public class LlcDocumentationData
{
    /// <summary>
    /// Does the registrant have the required documentation available to upload?
    /// </summary>
    public bool? HasRequiredDocumentation { get; set; }

    // --- "Yes" branch ---

    /// <summary>Uploaded file name (for display/tracking purposes).</summary>
    public string? UploadedFileName { get; set; }

    /// <summary>Uploaded file content bytes.</summary>
    public byte[]? UploadedFileContent { get; set; }

    /// <summary>Uploaded file MIME content type.</summary>
    public string? UploadedFileContentType { get; set; }

    // --- "No" branch ---

    /// <summary>
    /// The single reason the documentation cannot be submitted (mutually exclusive).
    /// </summary>
    public NoDocReason? NoDocumentationReason { get; set; }

    /// <summary>
    /// When do you plan to submit your application to the IRS? (must be a future date)
    /// Conditional on <see cref="NoDocumentationReason"/> == <see cref="NoDocReason.HaventApplied"/>.
    /// </summary>
    public DateOnly? PlannedSubmissionDate { get; set; }

    /// <summary>
    /// What date was the application submitted to the IRS? (must be a past date)
    /// Conditional on <see cref="NoDocumentationReason"/> == <see cref="NoDocReason.AppliedNoDecision"/>.
    /// </summary>
    public DateOnly? ApplicationSubmittedDate { get; set; }

    /// <summary>
    /// The registrant acknowledges they will submit the required documentation.
    /// </summary>
    public bool AcknowledgeSubmitDocumentation { get; set; }
}
