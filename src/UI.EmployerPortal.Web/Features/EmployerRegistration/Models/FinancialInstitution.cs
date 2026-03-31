namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// For the Unemployment Insurance Subjectivity page/  This contains the data for the
/// employer's Financial Institution
/// </summary>
public class FinancialInstitution
{
    /// <summary>
    /// Name of the Financial Institution
    /// </summary>
    [Required(ErrorMessage = "Financial Institution name is required.")]
    public string? Name { get; set; }
    /// <summary>
    /// Country
    /// </summary>
    [Required(ErrorMessage = "Country is required.")]
    public string? Country { get; set; }
    /// <summary>
    /// Address Line 1
    /// </summary>
    [Required(ErrorMessage = "Address is required.")]
    public string? AddressLine1 { get; set; }
    /// <summary>
    /// Address Line 2 (Optional)
    /// </summary>
    public string? AddressLine2 { get; set; }
    /// <summary>
    /// City
    /// </summary>
    [Required(ErrorMessage = "City is required.")]
    public string? City { get; set; }
    /// <summary>
    /// State
    /// </summary>
    [Required(ErrorMessage = "State is required.")]
    public string? State { get; set; }
    /// <summary>
    /// Zip Code
    /// </summary>
    [Required(ErrorMessage = "Zip Code is required.")]
    public string? ZipCode { get; set; }
    /// <summary>
    /// Zip Code Extenstion (Optional)
    /// </summary>
    public string? ZipCodeExtension { get; set; }
}
