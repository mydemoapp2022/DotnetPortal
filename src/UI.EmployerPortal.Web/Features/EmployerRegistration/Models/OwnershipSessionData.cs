namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Ownership data to store in session
/// </summary>
public class OwnershipSessionData
{
    /// <summary>
    /// OwnershipType
    /// </summary>
    public string OwnershipType { get; set; } = string.Empty;
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
}
