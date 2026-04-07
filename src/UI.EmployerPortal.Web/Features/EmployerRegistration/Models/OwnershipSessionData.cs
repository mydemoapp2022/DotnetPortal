namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Ownership data to store in session
/// </summary>
public class OwnershipSessionData
{
    /// <summary>
    /// OwnershipType
    /// </summary>
    public OwnershipType OwnershipType { get; set; } = OwnershipType.None;
    /// <summary>
    /// IsOutsideUSA
    /// </summary>
    public bool IsOutsideUSA { get; set; }

    /// <summary>
    /// RegistrationState For member-based forms (LLC, LLP, LP, Partnership)
    /// </summary>
    public string? RegistrationState { get; set; }
    /// <summary>
    /// Members
    /// </summary>
    public List<OwnerMember>? Members { get; set; }
    /// <summary>
    /// MoreThanFive
    /// </summary>
    public bool? MoreThanFive { get; set; }

    /// <summary>
    /// IncorporationState - For corporation forms
    /// </summary>
    public string? IncorporationState { get; set; }
    /// <summary>
    /// ForeignCountry
    /// </summary>
    public string? ForeignCountry { get; set; }
    /// <summary>
    /// Officers
    /// </summary>
    public List<OwnerMember>? Officers { get; set; }

    /// <summary>
    /// Owner For sole proprietorship
    /// </summary>
    public OwnerMember? Owner { get; set; }

    /// <summary>
    /// Limited Partnership Name (for LP type)
    /// </summary>
    public string? LimitedPartnershipName { get; set; }

    /// <summary>
    /// General Partner (for LP type)
    /// </summary>
    public OwnerMember? GeneralPartner { get; set; }

    /// <summary>
    /// Decedent (for Estate type)
    /// </summary>
    public OwnerMember? Decedent { get; set; }

    /// <summary>
    /// Personal Representative (for Estate type)
    /// </summary>
    public OwnerMember? PersonalRepresentative { get; set; }

    /// <summary>
    /// Corporate Officer Services data (UCT-10056-E) — for Corporation and LLCCorporation
    /// when registrant has not paid and does not expect to pay employees
    /// </summary>
    public CorporateOfficerServicesData? CorporateOfficerServices { get; set; }

    /// <summary>
    /// LLC Corporation documentation data — for LLCCorporation type
    /// </summary>
    public LlcDocumentationData? LlcDocumentation { get; set; }

    /// <summary>
    /// Qualified Settlement Fund (QSF) questionnaire data — for QSF ownership type
    /// when registrant has paid or will pay wages.
    /// </summary>
    public QualifiedSettlementFundModel? QualifiedSettlementFund { get; set; }
}
