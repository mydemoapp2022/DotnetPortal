using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Data for the Qualified Settlement Fund (QSF) questionnaire section.
/// </summary>
public class QualifiedSettlementFundModel
{
    /// <summary>
    /// Were the payments for services performed in Wisconsin?
    /// </summary>
    public bool? PaymentsForServices { get; set; }

    // --- "Yes" branch ---

    /// <summary>Legal name of the entity that received the services.</summary>
    [Required(ErrorMessage = "Legal name is required")]
    public string? EntityLegalName { get; set; }

    /// <summary>Federal Employer Identification Number of the entity.</summary>
    [Required(ErrorMessage = "Federal ID Number is required")]
    public string? FederalIdNumber { get; set; }

    /// <summary>Wisconsin UI Account Number (optional).</summary>
    public string? WisconsinUiAccountNumber { get; set; }

    /// <summary>Wisconsin address where services were performed.</summary>
    public QsfServiceAddress ServiceAddress { get; set; } = new();

    // --- "No" branch ---

    /// <summary>Reason for payments when not for services in Wisconsin.</summary>
    public string? PaymentReason { get; set; }

    // --- File upload (always shown) ---

    /// <summary>Uploaded Settlement Agreement file name.</summary>
    public string? UploadedFileName { get; set; }

    /// <summary>Uploaded Settlement Agreement file content bytes.</summary>
    public byte[]? UploadedFileContent { get; set; }

    /// <summary>Uploaded Settlement Agreement file MIME content type.</summary>
    public string? UploadedFileContentType { get; set; }

    /// <summary>
    /// Registrant acknowledges they will provide the Settlement Agreement documentation later.
    /// When true, file upload is not required.
    /// </summary>
    public bool WillProvideDocumentationLater { get; set; }
}
