using System.ComponentModel.DataAnnotations;

namespace UI.EmployerPortal.Web.Features.EmployerRegistration.Models;

/// <summary>
/// Wisconsin address where services were performed for a Qualified Settlement Fund.
/// </summary>
public class QsfServiceAddress
{
    /// <summary>Address Line 1.</summary>
    [Required(ErrorMessage = "Address Line 1 is required")]
    public string? AddressLine1 { get; set; }

    /// <summary>Address Line 2 (optional).</summary>
    public string? AddressLine2 { get; set; }

    /// <summary>City.</summary>
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }

    /// <summary>State (defaults to Wisconsin).</summary>
    public string State { get; set; } = "Wisconsin";

    /// <summary>Zip code (5 digits).</summary>
    [Required(ErrorMessage = "Zip Code is required")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip Code must be 5 digits")]
    public string? ZipCode { get; set; }

    /// <summary>Zip extension (optional, 4 digits).</summary>
    public string? ZipExtension { get; set; }
}
